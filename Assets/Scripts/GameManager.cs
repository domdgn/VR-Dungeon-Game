using ProceduralDungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DungeonGenerator generator;
    public StartTrigger startTrig;
    private Timer timer;
    private bool isDungeonGenerating = false;
    private bool isGameActive = false;
    private bool playerAtStart = true;

    public UnityEvent onGameBegin = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();

    private void Awake()
    {
        if (generator == null)
        {
            generator = FindObjectOfType<DungeonGenerator>();
            if (generator == null)
            {
                Debug.LogError("DungeonGenerator not found in the scene!");
            }
        }

        if (generator != null)
        {
            generator.onGenerationStarted.AddListener(DungeonGenerating);
            generator.onGenerationCompleted.AddListener(DungeonFinished);
        }

        timer = GetComponent<Timer>();
        if (timer == null)
        {
            Debug.LogError("Timer component not found on GameManager!");
        }
        else
        {
            timer.onTimerEnd.AddListener(TimerRunOut);
        }

        if (startTrig != null)
        {
            startTrig.onStartTriggerEnter.AddListener(OnStartEnter);
            startTrig.onStartTriggerExit.AddListener(OnStartExit);
        }
        else
        {
            Debug.LogError("StartTrigger reference is missing!");
        }
    }

    private void DungeonGenerating()
    {
        isDungeonGenerating = true;
    }

    private void DungeonFinished()
    {
        isDungeonGenerating = false;
    }

    public void StartGame()
    {
        if (!isGameActive && !isDungeonGenerating)
        {
            StopAllCoroutines();
            StartCoroutine(StartGameCoroutine());
        }
    }

    public IEnumerator StartGameCoroutine()
    {
        while (isDungeonGenerating)
        {
            yield return null;
        }

        isGameActive = true;

        if (timer != null)
        {
            timer.StartTimer();
        }

        onGameBegin.Invoke();
        Debug.Log("Game started!");
    }

    private void TimerRunOut()
    {
        if (isGameActive)
        {
            StartCoroutine(EndGame());
        }
    }

    private void OnStartEnter()
    {
        playerAtStart = true;
    }

    private void OnStartExit()
    {
        playerAtStart = false;
    }

    public IEnumerator EndGame()
    {
        if (isGameActive)
        {
            isGameActive = false;
            onGameOver.Invoke();

            Debug.Log("Game over!");

            if (generator != null && playerAtStart)
            {
                timer.ResetTimer();
                generator.ResetDungeon();

                while (isDungeonGenerating)
                {
                    yield return null;
                }
            }

            else if (!playerAtStart)
            {
                SceneManager.LoadScene("FailMenu");
            }
        }
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public bool IsDungeonGenerating()
    {
        return isDungeonGenerating;
    }
}