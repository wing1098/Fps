using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerCharacterController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Transform debugHitPointTransform;
    [SerializeField] private Transform hookshotTransform;
    private CharacterController characterController;
    private float cameraVerticalAngle;
    private float characterVelocityY;
    private Vector3 characterVelocityMomentum;
    private Camera playerCamera;
    //private bool isRunning;
    private float moveSpeed;

    [Header("Recoil Setting")]
    public float recoilSpeed = 5f;
    static public float upRecoil = 0;
    static public float sideRecoil = 0;

    [Header("Hook Settiing"), SerializeField]
    private bool isCoolDowning;
    private Vector3 hookShotPosition;
    private float hookShotSize;
    public float hookShotThrowSpeed = 70f;
    public float hookRange = 100f;
    public float hookShotSpeedMax = 40f;
    public float hookShotSpeedMin = 10f;
    //加速エフェクト
    private const float NORMAL_FOV = 60F;
    private const float HOOKSHOT_FOV = 100F;
    private CameraFov cameraFov;
    private ParticleSystem speedtLinesPS;

    public Image[] image;
    private float hookReloadTime = 3;
    static public float hookRT;
    [Header("State"), SerializeField]
    private State state;

    private enum State
    {
        Normal,
        HookshotThrow,
        HookshotFlyingPlayer,
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        cameraFov = playerCamera.GetComponent<CameraFov>();
        speedtLinesPS = transform.Find("Main Camera").Find("SpeedLinesPS").GetComponent<ParticleSystem>();

        Cursor.lockState = CursorLockMode.Locked;
        state = State.Normal;
        hookshotTransform.gameObject.SetActive(false);

        //isRunning = false;
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.Normal:
                HandleCharaterLook();
                HandleCharaterMovement();
                HandleHookShotStart();
                break;
            case State.HookshotThrow:
                HandleHookShotThrown();
                HandleCharaterLook();
                HandleCharaterMovement();
                break;
            case State.HookshotFlyingPlayer:
                HandleCharaterLook();
                HandleHookMovement();
                break;
        }

        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit);

        if (raycastHit.distance <= hookRange && !isCoolDowning)
            CanHookColor();
        else
            CantHookColor();
    }

    private void HandleCharaterLook()
    {
        float mouseX = sideRecoil + Input.GetAxisRaw("Mouse X");
        float mouseY = upRecoil + Input.GetAxisRaw("Mouse Y");
        upRecoil = 0;
        sideRecoil = 0;

        transform.Rotate(new Vector3(0f, mouseX * mouseSensitivity, 0f), Space.Self);

        cameraVerticalAngle -= mouseY * mouseSensitivity;

        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -90f, 90);

        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //
        //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //playerBody.Rotate(Vector3.up * mouseX);
    }

    private void HandleCharaterMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Running())
        {
            moveSpeed = 22f;
        }
        if (!Running())
        {
            moveSpeed = 12f;
        }

        Vector3 charaterVelocity = ((transform.right * moveX) + (transform.forward * moveZ)) * moveSpeed;

        if (characterController.isGrounded)
        {
            characterVelocityY = 0f;
            if (TestInputJump())
            {
                float jumpSpeed = 30f;
                characterVelocityY = jumpSpeed;
            }
        }

        float gravityDownForce = -60f;
        characterVelocityY += gravityDownForce * Time.deltaTime;

        charaterVelocity.y = characterVelocityY;

        //apply Momentum
        charaterVelocity += characterVelocityMomentum;

        characterController.Move(charaterVelocity * Time.deltaTime);

        //Dampen monentum
        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < 0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }
    }

    private void HandleHookShotStart()
    {
        if (TestInputDownHookShot())
        {
            Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit);

            //raycast ignores skybox
            if (raycastHit.transform == null)
            {
                return;
            }

            if(raycastHit.distance <= hookRange && !isCoolDowning)
            {
                //hit something
                debugHitPointTransform.transform.position = raycastHit.point;
                hookShotPosition = raycastHit.point;
                hookShotSize = 0f;
                hookshotTransform.gameObject.SetActive(true);
                hookshotTransform.localScale = Vector3.zero;
                state = State.HookshotThrow;

                StartCoroutine(HookCoolDown());
            }

            //hit something
            //debugHitPointTransform.transform.position = raycastHit.point;
            //hookShotPosition = raycastHit.point;
            //hookShotSize = 0f;
            //hookshotTransform.gameObject.SetActive(true);
            //hookshotTransform.localScale = Vector3.zero;
            //state = State.HookshotThrow;

        }
    }

    private void HandleHookShotThrown()
    {
        hookshotTransform.LookAt(hookShotPosition);

        hookShotSize += hookShotThrowSpeed * Time.deltaTime;
        hookshotTransform.localScale = new Vector3(1, 1, hookShotSize);

        if (hookShotSize >= Vector3.Distance(transform.position, hookShotPosition))
        {
            state = State.HookshotFlyingPlayer;
            cameraFov.SetCameraFov(HOOKSHOT_FOV);
            speedtLinesPS.Play();
        }
    }

    private void ResetGravityEffect()
    {
        characterVelocityY = 0f;
    }

    private void HandleHookMovement()
    {
        hookshotTransform.LookAt(hookShotPosition);

        Vector3 hoolShotDir = (hookShotPosition - transform.position).normalized;

        float hookShotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookShotPosition), hookShotSpeedMin, hookShotSpeedMax);
        float hookShotSpeedMultiplier = 2f;
        //move player
        characterController.Move(hoolShotDir * hookShotSpeed * hookShotSpeedMultiplier * Time.deltaTime);


        float reachedHookShotPositonDistance = 2f;
        if (Vector3.Distance(transform.position, hookShotPosition) < reachedHookShotPositonDistance)
        {
            //Reached Hook Position
            StopHookShot();
        }

        if (TestInputDownHookShot())
        {
            //cancel Hookshot
            StopHookShot();
        }

        if (TestInputJump())
        {
            //cancel with jump
            float momentumExtraSpeed = 7f;
            characterVelocityMomentum = hoolShotDir * hookShotSpeed * momentumExtraSpeed;

            float jumpSpeed = 40f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;

            StopHookShot();
        }
    }

    private void StopHookShot()
    {
        state = State.Normal;
        ResetGravityEffect();
        hookshotTransform.gameObject.SetActive(false);
        cameraFov.SetCameraFov(NORMAL_FOV);
        speedtLinesPS.Stop();
    }

    public static void AddRecoil(float up, float side)
    {
        upRecoil += up;
        sideRecoil += side;
    }

    private bool TestInputDownHookShot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    IEnumerator HookCoolDown()
    {
        Debug.Log("Red now");
        isCoolDowning = true;

        yield return new WaitForSeconds(hookReloadTime);

        isCoolDowning = false;
        Debug.Log("White now");
    }
    void CanHookColor()
    {
        image[0].color = new Color(0, 0, 1, 0.5f);
        image[1].color = new Color(0, 0, 1, 0.5f);
        image[2].color = new Color(0, 0, 1, 0.5f);
        image[3].color = new Color(0, 0, 1, 0.5f);
    }

    void CantHookColor()
    {
        image[0].color = new Color(1, 0, 0, 0.5f);
        image[1].color = new Color(1, 0, 0, 0.5f);
        image[2].color = new Color(1, 0, 0, 0.5f);
        image[3].color = new Color(1, 0, 0, 0.5f);
    }

    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool Running()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
}
