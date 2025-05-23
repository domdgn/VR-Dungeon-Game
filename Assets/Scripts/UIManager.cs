using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        moneyText.text = $"�{totalValueRecovered:N2}";

        if (itemCountText != null)
        {
            itemCountText.text = $"({totalItemCount} items)";
        }
    }

    public void ChangeSceneToGame()
    {
        if (MoneyManager.Instance != null)
        {
            Destroy(MoneyManager.Instance.gameObject);
        }
        SceneManager.LoadScene("Game");
    }

    public void ChangeSceneToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void ChangeSceneToWin()
    {
        SceneManager.LoadScene("WinMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
