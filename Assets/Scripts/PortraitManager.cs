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
        mainFolderPath = Path.Combine(Application.dataPath, relativeFolderPath); // 상대 경로를 절대 경로로 변환
        characterFolderNames = Directory.GetFiles(mainFolderPath); //캐릭터 폴더 이름 

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
        texture.LoadImage(fileData); // 파일 데이터로부터 텍스처 로드
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
