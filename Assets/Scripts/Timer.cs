using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timerLength = 180f;
    [SerializeField] Color warningColor = Color.yellow;
    [SerializeField] Color criticalColor = Color.red;
    [SerializeField] float warningThreshold = 30f;
    [SerializeField] float criticalThreshold = 10f;

    private float remainingTime;
    private Coroutine timerCoroutine;
    public GameManager gameMgr;
    public UnityEvent onTimerEnd = new UnityEvent();
    private Color defaultTextColor;
    private bool isRunning = false;

    private void Awake()
    {
        remainingTime = timerLength;

        if (timerText != null)
        {
            defaultTextColor = timerText.color;
        }
        else
        {
            Debug.LogError("Timer text reference is missing!");
        }

        if (gameMgr == null)
        {
            gameMgr = FindObjectOfType<GameManager>();
            if (gameMgr == null)
            {
                Debug.LogError("Game Manager not found!");
            }
        }

        if (gameMgr != null)
        {
            gameMgr.onGameBegin.AddListener(StartTimer);
            gameMgr.onGameOver.AddListener(StopTimer);
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

        // Update color based on remaining time
        if (remainingTime <= criticalThreshold)
        {
            timerText.color = criticalColor;
        }
        else if (remainingTime <= warningThreshold)
        {
            timerText.color = warningColor;
        }
        else
        {
            timerText.color = defaultTextColor;
        }
    }

    private IEnumerator TimerCountdown()
    {
        isRunning = true;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
                remainingTime = 0;

            UpdateTimerDisplay();

            if (remainingTime <= 0)
            {
                onTimerEnd.Invoke();
                isRunning = false;
                break;
            }

            yield return null;
        }

        isRunning = false;
    }

    public void StartTimer()
    {
        if (isRunning)
        {
            StopTimer();
        }

        ResetTimer();
        timerCoroutine = StartCoroutine(TimerCountdown());
        Debug.Log("Timer started");
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            isRunning = false;
            Debug.Log("Timer stopped");
        }
    }

    public void PauseTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            isRunning = false;
            Debug.Log("Timer paused");
        }
    }

    public void ResumeTimer()
    {
        if (!isRunning && remainingTime > 0)
        {
            timerCoroutine = StartCoroutine(TimerCountdown());
            Debug.Log("Timer resumed");
        }
    }

    public void SetTimerLength(float length)
    {
        timerLength = length;
        if (!isRunning)
        {
            remainingTime = timerLength;
            UpdateTimerDisplay();
        }
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public void ResetTimer()
    {
        remainingTime = timerLength;
        UpdateTimerDisplay();
        Debug.Log("Timer reset");
    }
}