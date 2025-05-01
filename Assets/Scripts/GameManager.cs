using ProceduralDungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public DungeonGenerator generator;

    private Timer timer;
    private bool isDungeonGenerating = false;
    private bool isGameActive = false;

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
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(TimerRunOut);
        }
        else
        {
            Debug.LogError("Timer component not found on GameManager object!");
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
        if (!isGameActive)
        {
            StopAllCoroutines();
            StartCoroutine(StartGameCoroutine());
        }
    }

    public IEnumerator StartGameCoroutine()
    {
        if (generator != null)
        {
            generator.ResetDungeon();

            while (isDungeonGenerating)
            {
                yield return null;
            }
        }

        isGameActive = true;
        onGameBegin.Invoke();
    }

    private void TimerRunOut()
    {
        if (isGameActive)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        if (isGameActive)
        {
            isGameActive = false;
            onGameOver.Invoke();
        }
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }
}