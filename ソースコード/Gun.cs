using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Gun Info")]
    public float headDamage = 108.0f;
    public float bodyDamage = 33.0f;
    public float armDamage = 27.0f;
    public float legDamage = 26.0f;
    public float range = 100f;
    public float fireRange = 15f;
    public float impactForce = 60f;

    public int maxAmmo = 30;
    public static int textMaxAmmo;

    [SerializeField]
    private int currentAmmo = -1;
    public float reloadTime;
    private bool isReloading = false;
    public static int textAmmo;

    [SerializeField]
    private float nextTimeToFire = 0f;
    Animator anim;

    [Header("Recoil System")]
    public float recoilBuffer = 5f;
    public float maxUpRecoil;
    public float minUpRecoil;
    public float maxSideRecoil;
    public float minSideRecoil;
    private AudioSource sound;

	[Header("Gun Effect")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("System")]
    public Camera fpsCamera;
    [SerializeField]
    private bool isRunning;
    public Image[] image;
    private float chageColor = .2f;

    private void Awake()
    {
		anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();

        textMaxAmmo = maxAmmo;

        if (currentAmmo == -1)
        {
            currentAmmo = maxAmmo;
        }
    }

    void Update()
    {

        if (isReloading)
        {
            //Get reload anim delay
            reloadTime = anim.GetCurrentAnimatorStateInfo(0).length;
            return;
        }

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRange;
            Shoot();
        }

        if(Input.GetKey(KeyCode.W) && !isRunning || 
           Input.GetKey(KeyCode.A) && !isRunning ||
           Input.GetKey(KeyCode.S) && !isRunning ||
           Input.GetKey(KeyCode.D) && !isRunning)
            { 
                anim.SetBool("Walk", true);
            }
        else
        {
            anim.SetBool("Walk", false);
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if(isRunning == true)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }

        textAmmo = currentAmmo;
    }

    void Shoot()
    {
        muzzleFlash.Play();

        currentAmmo--;

        PlayerMovement.AddRecoil( Random.Range(minUpRecoil, maxUpRecoil) / recoilBuffer, Random.Range(minSideRecoil, maxSideRecoil) / recoilBuffer);
        sound.Play();
        anim.Play("Fire", 0, 0f);

        RaycastHit hit;
        if(Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.tag);

            //Hit Normal Enemy
            {
                if (hit.transform.tag == "Normal_EnemyHead")
                {
                    Target target = hit.transform.GetComponentInParent<Target>();
                    target.TakeDamage(headDamage);
                    StartCoroutine(ChageCrossHairColor());
                    Debug.Log("Hit Norma Head");

                    if (target.health <= 0)
                    {
                        hit.transform.gameObject.SetActive(false);
                    }
                }
                else if (hit.transform.tag == "Normal_EnemyBody")
                {
                    Target target = hit.transform.GetComponentInParent<Target>();
                    target.TakeDamage(bodyDamage);
                    StartCoroutine(ChageCrossHairColor());

                    Debug.Log("Hit Norma Body");
                }
            }

            //Hit Armoured Enemy
            {
                if (hit.transform.tag == "Armoured_EnemyHead")
                {
                    Target target = hit.transform.GetComponentInParent<Target>();
                    target.TakeDamage(headDamage / 2);
                    StartCoroutine(ChageCrossHairColor());

                    Debug.Log("Hit Armoured Head");

                    if (target.health <= 0)
                    {
                        hit.transform.gameObject.SetActive(false);
                    }
                }
                else if (hit.transform.tag == "Armoured_EnemyBody")
                {
                    Target target = hit.transform.GetComponentInParent<Target>();
                    target.TakeDamage(bodyDamage / 2);
                    StartCoroutine(ChageCrossHairColor());

                    Debug.Log("Hit Armoured Body");
                }
            }

            //Add impact to dead body
            if (hit.rigidbody != null)
            {
                var impactTarget = hit.rigidbody;
                var impact = fpsCamera.transform.forward * impactForce;
                impactTarget.AddForce(impact, ForceMode.VelocityChange);
                //hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);

        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reloading");

        anim.Play("Reload Out Of Ammo", 0, 0f);

        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;

        isReloading = false;
    }

    IEnumerator ChageCrossHairColor()
    {
        Debug.Log("Red now");
        ChageToRed();
        
        yield return new WaitForSeconds(chageColor);

        Debug.Log("White now");
        ChageToWhite();
    }
    void ChageToRed()
    {
        image[0].color = Color.red;
        image[1].color = Color.red;
        image[2].color = Color.red;
        image[3].color = Color.red;
    }

    void ChageToWhite()
    {
        image[0].color = Color.white;
        image[1].color = Color.white;
        image[2].color = Color.white;
        image[3].color = Color.white;
    }
}
