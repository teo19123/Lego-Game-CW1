using TMPro;
using UnityEngine;

public class DeliveryTargetHUD : MonoBehaviour
{
    public TMP_Text targetText;
    public string targetHouse = "A";

    void Update()
    {
        targetText.text = "HOUSE " + targetHouse;
    }

    public void SetTarget(string house)
    {
        targetHouse = house;
    }
}
