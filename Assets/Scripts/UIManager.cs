using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI itemCountText;

    private float totalValueRecovered;
    private int totalItemCount;

    private void Awake()
    {
        UpdateMoneyText(); 
    }

    public void UpdateMoneyText()
    {
        totalValueRecovered = MoneyManager.Instance.GetValueRecovered();
        totalItemCount = MoneyManager.Instance.GetItemCount();

        moneyText.text = $"£{totalValueRecovered:N2}";

        if (itemCountText != null)
        {
            itemCountText.text = $"({totalItemCount} items)";
        }
    }
}
