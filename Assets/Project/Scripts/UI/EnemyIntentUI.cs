using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIntentUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI damageText;

    public void SetIntent(Sprite attackIcon, int damage)
    {
        if (icon != null)
            icon.sprite = attackIcon;

        if (damageText != null)
            damageText.text = damage.ToString();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}