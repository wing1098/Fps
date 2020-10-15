using UnityEngine;
using System.Collections;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
    [Header("Info")]
    public GameObject bullet;
    public GameObject muzzleFire;
    public GameObject ammuntionDisplay;
    public Camera cam;
    public Transform attackPoint;
    [SerializeField]
    private Animator anim;
    public Rigidbody playerRb;

    [Header("Gun Setting")]
    public float shootForce;
    public float upwardForce;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    bool shooting, readyToShoot, reloading;
    public float recoilForce;
    private bool isReloading = false;
    public bool allowInvoke = true;


    [Header("Recoil System")]
    public float recoilBuffer = 5f;
    public float maxUpRecoil;
    public float minUpRecoil;
    public float maxSideRecoil;
    public float minSideRecoil;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cam  = Camera.main;
        playerRb = GameObject.FindGameObjectWithTag("PlayerMain").GetComponent<Rigidbody>();
        ammuntionDisplay = GameObject.Find("BulletTextMesh");
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        if (isReloading)
        {
            //Get reload anim delay
            reloadTime = anim.GetCurrentAnimatorStateInfo(0).length;
            return;
        }

        MyInput();

        if(ammuntionDisplay != null)
        {
            ammuntionDisplay.GetComponent<TextMeshProUGUI>().SetText(bulletsLeft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);
        }

    }

    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) 
        {
            StartCoroutine(Reload());
        }

        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            StartCoroutine(Reload());
        }

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        PlayerMovement.AddRecoil(Random.Range(minUpRecoil, maxUpRecoil) / recoilBuffer, Random.Range(minSideRecoil, maxSideRecoil) / recoilBuffer);

        //anim.SetTrigger("Shoot");
        //anim.Play("GunRecoil", 0, 0f);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse) ;
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

        float random = Random.Range(-10.0f, 10.0f);
        currentBullet.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 36);

        if(muzzleFire != null)
        {
            Instantiate(muzzleFire, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        if(allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more then one bulletPreTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    //private void Reload()
    //{
    //    //reloadTime = anim.GetCurrentAnimatorStateInfo(0).length;
    //    
    //    anim.Play("GunReload", 0, 0f);
    //    
    //    reloadTime = anim.GetCurrentAnimatorStateInfo(0).length;
//
    //    Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);
//
    //    reloading = true;
    //    Invoke("ReloadFinished", reloadTime);
    //}

    IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reloading");

        anim.Play("GunReload", 0, 0f);

        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(reloadTime);

        bulletsLeft = magazineSize;

        isReloading = false;
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
