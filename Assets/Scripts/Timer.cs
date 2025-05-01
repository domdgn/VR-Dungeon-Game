using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timerLength = 180f;

    private float remainingTime;
    private Coroutine timerCoroutine;

    public GameManager gameMgr;
    public UnityEvent onTimerEnd = new UnityEvent();

    private void Awake()
    {
        remainingTime = timerLength;

        if (gameMgr != null)
        {
            gameMgr.onGameBegin.AddListener(BeginTimer);
            gameMgr.onGameOver.AddListener(ResetTimer);
        }
        else
        {
            Debug.LogError("Game Manager not assigned to Timer!");
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

    private IEnumerator TimerCountdown()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0)
                remainingTime = 0;

            UpdateTimerDisplay();

            if (remainingTime <= 0)
            {
                onTimerEnd.Invoke();
                break;
            }

            yield return null;
        }
    }

    private void BeginTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        ResetTimer();

        timerCoroutine = StartCoroutine(TimerCountdown());
    }

    public void SetTimerLength(float length)
    {
        timerLength = length;
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    private void ResetTimer()
    {
        remainingTime = timerLength;
        UpdateTimerDisplay();
    }
}