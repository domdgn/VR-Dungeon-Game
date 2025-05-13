using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchHolderSpawn : MonoBehaviour
{
    public GameObject torch;

    private void Awake()
    {
        if (Random.Range(0, 3) == 0)
        {
            torch.SetActive(true);
        }
        else
        {
            if (torch != null)
            {
                Destroy(torch);
            }
        }
    }
}