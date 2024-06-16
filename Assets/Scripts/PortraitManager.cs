using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    //초상화 메인 폴더 상대경로
    public string relativeFolderPath = string.Empty;
    //초상화 메인 폴더 절대경로
    [SerializeField] private string mainFolderPath;


    //폴더 이름 앞 번호로 배열의 위치에 할당된 폴더 이름들 
    [SerializeField] private string[] charFolderNames;
    //이미지 배열
    public Sprite[,] imageArray;


    //캐릭터 폴더 룩업 할당하기
    void LoadFolders()
    {
        string[] charFolderNamesTemp;

        mainFolderPath = Path.Combine(Application.dataPath, relativeFolderPath); // 상대 경로를 절대경로로 변환
        charFolderNamesTemp = Directory.GetFiles(mainFolderPath); //캐릭터 폴더 이름 

        //캐릭터 이미지 배열, 캐릭터 폴더 이름 룩업의 길이를 마지막 초상화 ID의 크기만큼 할당
        //폴더의 마지막 초상화 ID를 구하기
        string[] lastSplit = Path.GetFileNameWithoutExtension(charFolderNamesTemp[charFolderNamesTemp.Length - 1]).Split('-');
        int lastPortraitID;
        //초상화 ID만큼 크기 할당하기 (만약 마지막 초상화 ID에 할당할 숫자를 찾을 수 없을 시 10000크기로 할당)
        if (int.TryParse(lastSplit[0], out lastPortraitID))
        {
            charFolderNames = new string[lastPortraitID + 1];
            imageArray = new Sprite[lastPortraitID + 1, 100];
        }
        else
        {
            Debug.LogWarning($"초상화 폴더의 마지막 이름, \"{charFolderNamesTemp[charFolderNamesTemp.Length - 1]}\"이 올바르지 않은 형식 이므로 이미지 배열, 폴더 이름 배열의 크기가 10000만큼 할당되었습니다.");
            charFolderNames = new string[10000];
            imageArray = new Sprite[10000, 100];
        }


        //캐릭터 폴더 이름 룩업에 폴더 이름 할당하기
        for (int i = 0; i < charFolderNamesTemp.Length; i++)
        {
            //캐릭터 ID 구하고 할당하기
            string folderName = Path.GetFileNameWithoutExtension(charFolderNamesTemp[i]);
            string[] splitName = folderName.Split('-');

            int charPortraitID;

            if (int.TryParse(splitName[0], out charPortraitID))
            {
                charFolderNames[charPortraitID] = folderName;
            }
            else
            {
                Debug.LogWarning($"초상화 폴더의 이름,{folderName} 이 올바르지 않은 형식 이므로, 해당 폴더 이름을 배열에 할당 할 수 없습니다.");
            }
        }
    }

    void LoadImages()
    {
        for(int i = 0; i < charFolderNames.Length; i++)
        {
            if (charFolderNames[i] != string.Empty)
            {
                //TODO: charFolderNameLookUps에서 폴더 이름을 가져와서 mainFolderPath와 합쳐 하려는 imageArray에 할당 하려는 캐릭터의 폴더 경로를 얻은 후,
                //폴더 내 이미지들을 가져와서 imageArray에 imageArray[캐릭터 번호][표정 번호]위치에 할당하는 스크립트 작성
                string path = mainFolderPath + '\\' + charFolderNames[i];

                string[] ImagePaths = Directory.GetFiles(path, "*.png"); // 폴더 내의 모든 PNG 파일 이름 가져오기
                string[] imageNames = new string[ImagePaths.Length];     // 폴더 내의 이미지 이름들

                //할당하기
                for (int o = 0; o < ImagePaths.Length; o++)
                {
                    int assignLocation = Int32.Parse(Path.GetFileNameWithoutExtension(ImagePaths[o]).Split('-')[0]);

                    //이미지 로드
                    Texture2D texture = LoadTextureFromFile(ImagePaths[o]);
                    if(texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0, SpriteMeshType.FullRect, new Vector4(0, 0, texture.width, texture.height), false);
                        sprite.texture.filterMode = FilterMode.Point; // FilterMode를 None으로 설정

                        imageArray[i,assignLocation] = sprite;
                    }
                }

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
        LoadFolders();
        LoadImages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
