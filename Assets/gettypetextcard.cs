using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gettypetextcard : MonoBehaviour
{
    private TMP_Text myRarity;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRarity = GetComponent<TMP_Text>();
        var parent = GetComponentInParent<Upgrade>();

        if (parent != null)
        {
            string rarityEnumString = parent.rarity.ToString();
            myRarity.text = rarityEnumString;
            ApplyRarityColor(parent.rarity);
        }
    }

    private void ApplyRarityColor(Upgrade.UpgradeRarity type)
    {
        switch (type)
        {
            case Upgrade.UpgradeRarity.SUPERRARE: myRarity.color = new Color(1f, 0.6470588f, 0f, 1f); break;//orange
            case Upgrade.UpgradeRarity.EPIC: myRarity.color = new Color(0.627f, 0.125f, 0.941f, 1f); break; // purple
            case Upgrade.UpgradeRarity.LEGENDARY: myRarity.color = Color.yellow; break;
        }
    }
}
