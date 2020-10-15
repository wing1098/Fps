using System.Collections;
using UnityEngine;
using TMPro;

public class AmmoCounterManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public int currentBullet;

    void Update()
    {
        

        ammoText.text = Gun.textAmmo.ToString() + "/" + Gun.textMaxAmmo.ToString();
    }
}
