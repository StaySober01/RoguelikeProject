using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIntentUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Sprite attackIcon;
    [SerializeField] private Sprite blockIcon;
    [SerializeField] private Sprite statusIcon;

    public void SetIntent(EnemyActionType type, int amount)
    {
        switch (type)
        {
            case EnemyActionType.Attack:
                icon.sprite = attackIcon;
                break;

            case EnemyActionType.GainBlock:
                icon.sprite = blockIcon;
                break;

            case EnemyActionType.ApplyVulnerable:
                icon.sprite = statusIcon;
                break;
        }

        amountText.text = amount.ToString();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}