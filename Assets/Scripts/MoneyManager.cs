using System.Collections.Generic;
using UnityEngine;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    private UIManager uiMgr;
    private readonly HashSet<GameObject> collectedItems = new HashSet<GameObject>();
    private StartTrigger startTrig;
    private float totalValueRecovered = 0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        uiMgr = FindObjectOfType<UIManager>();
        startTrig = FindObjectOfType<StartTrigger>();
        if (startTrig != null)
        {
            startTrig.onTreasureTriggerEnter.AddListener(AddTreasureToList);
            startTrig.onTreasureTriggerExit.AddListener(RemoveTreasureFromList);
        }
        UpdateUI();
    }
    private void AddTreasureToList(GameObject item)
    {
        if (item != null && collectedItems.Add(item)) // HashSet.Add returns false if already present
        {
            Debug.Log($"[MoneyManager] Treasure added: {item.name}");
            UpdateValueRecovered();
            UpdateUI();
        }
    }
    private void RemoveTreasureFromList(GameObject item)
    {
        if (item != null && collectedItems.Remove(item))
        {
            Debug.Log($"[MoneyManager] Treasure removed: {item.name}");
            UpdateValueRecovered();
            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        if (uiMgr == null) { uiMgr = FindObjectOfType<UIManager>(); }
        if (uiMgr != null)
        {
            uiMgr.UpdateMoneyText();
        }
    }
    void UpdateValueRecovered()
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
    }
    public float GetValueRecovered()
    {
        return totalValueRecovered;
    }
    public int GetItemCount()
    {
        return collectedItems.Count;
    }
}