using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime = 60f ;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0 )
        {
            remainingTime -=Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        int Minutes = Mathf.FloorToInt(remainingTime / 60);
        int Seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);
    }
}
