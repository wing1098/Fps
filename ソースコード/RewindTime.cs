using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RewindTime : MonoBehaviour
{
    [SerializeField]
    public bool isRewinding = false;
    public float recordTime = 5f;
    public float effectV = 0f;
    Rigidbody rb;

    List<PointInTime> pointsInTime;
    private Camera playerCamera;
    private CameraFov cameraFov;
    private Volume v;
    private FilmGrain fg;
    private ChromaticAberration ca;

    private const float NORMAL_FOV = 60F;
    private const float HOOKSHOT_FOV = 100F;

    // Start is called before the first frame update
    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFov = playerCamera.GetComponent<CameraFov>();
        v = GameObject.Find("Global Volume").GetComponent<Volume>();
        v.profile.TryGet(out fg);
        v.profile.TryGet(out ca);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartRewinding();
        }
        if(Input.GetKeyUp(KeyCode.Q))
        {
            StopRewinding();
        }
    }
    
    private void FixedUpdate() {
        
        if(isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    void Rewind()
    {
        if(pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation; 
            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewinding();
        }
    }

    void Record()
    {
        if(pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }

    public void StartRewinding()
    {
        isRewinding = true;
        rb.isKinematic = true;
        for(effectV = 0; effectV < 10; effectV ++)
        {
            StartEffect();
        }
    }

    public void StopRewinding()
    {
        isRewinding = false;
        rb.isKinematic = false;

        for(effectV = 10; effectV > 0; effectV --)
        {
            StopEffect();
        }
    }


    void StartEffect()
    {
        ca.intensity.value = effectV / 10;
        fg.intensity.value = effectV / 10;
    }

    void StopEffect()
    {
        fg.intensity.value = effectV / 10;
        ca.intensity.value = effectV / 10;
    }
}
