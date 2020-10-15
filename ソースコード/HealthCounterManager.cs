using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthCounterManager : MonoBehaviour
{
    public TextMeshProUGUI HpText;

    void Update()
    {
        HpText.text = PlayerHealthManager.textHealth.ToString();

        if(PlayerHealthManager.textHealth <= 60)
        {
            HpText.color = Color.red;
        }
    }
}
