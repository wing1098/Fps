using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health Info")]
    public int staringHealth;
    public static int maxHealth;
    [SerializeField]
    public static int currentHealth;
    private int getHurtCount;
    public static int textHealth;

    public int staringDash;
    public int currentDash;

    public GameObject floatingTextPrefab;
    public GameObject damage_Image;

    void Start()
    {
        currentHealth = staringHealth;
        maxHealth = staringHealth;

        currentDash = staringDash;

        textHealth = maxHealth;
        //damage_Image = GetComponent<Image>();
        //取得預設顏色
    }

    void Update()
    {

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            HurtPlayer(10);
        }

        textHealth = currentHealth;
    }

    public void HurtPlayer(int damage)
    {
        getHurtCount = damage;
        currentHealth -= damage;

        if (floatingTextPrefab != null && currentHealth > 0)
        {
            ShowFloatingText();
        }

        //damage_Image.SetActive(true);
        Debug.Log("Red");
    }

    void ShowFloatingText()
    {
        var go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = getHurtCount.ToString();
    }
}
