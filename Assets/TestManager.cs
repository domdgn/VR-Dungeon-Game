using ProceduralDungeon;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public List<GameObject> disableWhileGenerating = new List<GameObject>();
    public DungeonGenerator generator;

    private void Start()
    {
        if (generator != null)
        {
            generator.onGenerationStarted.AddListener(DisableObjects);
            generator.onGenerationCompleted.AddListener(EnableObjects);
        }
        else
        {
            Debug.LogError("DungeonGenerator not assigned to TestManager!");
        }
    }

    public void DisableObjects()
    {
        Debug.Log("Disabling objects during dungeon generation");
        foreach (GameObject obj in disableWhileGenerating)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    public void EnableObjects()
    {
        foreach (GameObject obj in disableWhileGenerating)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        if (generator != null)
        {
            generator.onGenerationStarted.RemoveListener(DisableObjects);
            generator.onGenerationCompleted.RemoveListener(EnableObjects);
        }
    }
}