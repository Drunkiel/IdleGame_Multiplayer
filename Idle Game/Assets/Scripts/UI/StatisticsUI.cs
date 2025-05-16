using TMPro;
using UnityEngine;

public class StatisticsUI : MonoBehaviour
{
    public TMP_Text hitPointsText;
    public TMP_Text protectionText;
    public TMP_Text damageText;
    public TMP_Text criticalChanceText;
    public TMP_Text dodgeText;
    public TMP_Text damageReductionText;

    [Header("Attributes")]
    public TMP_Text strengthPointsText;
    public TMP_Text dexterityPointsText;
    public TMP_Text intelligencePointsText;
    public TMP_Text durabilityPointsText;
    public TMP_Text luckPointsText;
    public TMP_Text armorPointsText;

    public void AddAttributePoint(int index)
    {
        PlayerController.instance._entityInfo.AddPoint(index);
        PlayerController.instance.UpdateStats();
    }
}
