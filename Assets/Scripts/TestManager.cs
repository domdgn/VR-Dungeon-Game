using ProceduralDungeon;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public List<GameObject> disableWhileGenerating = new List<GameObject>();
    public GameManager mgr;

    private void Start()
    {
        if (mgr != null)
        {
            mgr.onGameOver.AddListener(EnableObjects);
        }
        else
        {
            Debug.LogError("DungeonGenerator not assigned to TestManager!");
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
        if (mgr != null)
        {
            mgr.onGameOver.RemoveListener(EnableObjects);
        }
    }
}