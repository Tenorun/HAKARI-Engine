using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    //0: 영어
    //1: 한국어
    public int languageVal = 0;

    void Awake()
    {
        //프레임 수 제한
        Application.targetFrameRate = 240;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        languageVal = 1;
    }
}
