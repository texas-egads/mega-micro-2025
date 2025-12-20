using TMPro;
using UnityEngine;

public class UpgradeTester : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool spacePressed = Input.GetButtonDown("Space");
        if(spacePressed)
        {
            Managers.__instance.upgradeManager?.DoUpgrade(PostUpgrade);
        }
    }

    void PostUpgrade()
    {
        text.text = Managers.__instance.upgradeManager?.GetText();
    }
}
