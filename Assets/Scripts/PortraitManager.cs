using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    public string relativeFolderPath = string.Empty;
    public Sprite[,] imageArray = new Sprite[10000, 100];


    [SerializeField] private string mainFolderPath;

    private string[] characterFolderNames = new string[100];
    private int[] folderPathNums = new int[100];

    
    void LoadFolders()
    {
        mainFolderPath = Path.Combine(Application.dataPath, relativeFolderPath); // ��� ��θ� ���� ��η� ��ȯ
        characterFolderNames = Directory.GetFiles(mainFolderPath); //ĳ���� ���� �̸� 

        for(int i = 0; i < characterFolderNames.Length; i++)
        {
            string folderName = Path.GetFileNameWithoutExtension(characterFolderNames[i]);
            string[] splitName = folderName.Split('-');

            int charPortraitID;

            if (int.TryParse(splitName[0], out charPortraitID))
            {
                characterFolderNames[charPortraitID] = folderName;
            }
        }
    }

    Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // ���� �����ͷκ��� �ؽ�ó �ε�
        return texture;
    }

    void Start()
    {
        for(int i = 0; i < characterFolderNames.Length; i++)
        {
            characterFolderNames[i] = "(none)";
            folderPathNums[i] = -1;
        }

        LoadFolders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
