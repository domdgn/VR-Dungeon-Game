using ProceduralDungeon;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI itemCountText;

    private readonly HashSet<GameObject> collectedItems = new HashSet<GameObject>();
    private StartTrigger startTrig;
    private float totalValueRecovered = 0f;

    private void Awake()
    {
        UpdateUI();

        startTrig = FindObjectOfType<StartTrigger>();
        if (startTrig == null)
        {
            Debug.LogError("StartTrigger not found in the scene!");
            return;
        }

        startTrig.onTreasureTriggerEnter.AddListener(AddTreasureToList);
        startTrig.onTreasureTriggerExit.AddListener(RemoveTreasureFromList);
    }

    private void AddTreasureToList(GameObject item)
    {
        if (item != null && collectedItems.Add(item)) // HashSet.Add returns false if already present
        {
            Debug.Log($"[MoneyManager] Treasure added: {item.name}");
            UpdateUI();
        }
    }

    private void RemoveTreasureFromList(GameObject item)
    {
        if (item != null && collectedItems.Remove(item))
        {
            Debug.Log($"[MoneyManager] Treasure removed: {item.name}");
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        totalValueRecovered = 0f;

        foreach (GameObject item in collectedItems)
        {
            if (item != null)
            {
                ObjectInformation objInfo = item.GetComponent<ObjectInformation>();
                if (objInfo != null)
                {
                    totalValueRecovered += objInfo.GetValue();
                }
            }
        }

        moneyText.text = $"�{totalValueRecovered:N2}";

        if (itemCountText != null)
        {
            itemCountText.text = $"Treasures: {collectedItems.Count}";
        }
    }
}
