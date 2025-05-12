using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchHolderSpawn : MonoBehaviour
{
    public GameObject torch;

    private void Awake()
    {
        SimpleXRCulling cullingScript = Camera.main.GetComponent<SimpleXRCulling>();

        if (cullingScript != null)
        {
            cullingScript.AddNeverCullObject(gameObject);
        }

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