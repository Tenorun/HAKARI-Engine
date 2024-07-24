using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    //0: ����
    //1: �ѱ���
    public int languageVal = 0;

    void Awake()
    {
        //������ �� ����
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
