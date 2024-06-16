using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    //�ʻ�ȭ ���� ���� �����
    public string relativeFolderPath = string.Empty;
    //�ʻ�ȭ ���� ���� ������
    [SerializeField] private string mainFolderPath;


    //���� �̸� �� ��ȣ�� �迭�� ��ġ�� �Ҵ�� ���� �̸��� 
    [SerializeField] private string[] charFolderNames;
    //�̹��� �迭
    public Sprite[,] imageArray;


    //ĳ���� ���� ��� �Ҵ��ϱ�
    void LoadFolders()
    {
        string[] charFolderNamesTemp;

        mainFolderPath = Path.Combine(Application.dataPath, relativeFolderPath); // ��� ��θ� �����η� ��ȯ
        charFolderNamesTemp = Directory.GetFiles(mainFolderPath); //ĳ���� ���� �̸� 

        //ĳ���� �̹��� �迭, ĳ���� ���� �̸� ����� ���̸� ������ �ʻ�ȭ ID�� ũ�⸸ŭ �Ҵ�
        //������ ������ �ʻ�ȭ ID�� ���ϱ�
        string[] lastSplit = Path.GetFileNameWithoutExtension(charFolderNamesTemp[charFolderNamesTemp.Length - 1]).Split('-');
        int lastPortraitID;
        //�ʻ�ȭ ID��ŭ ũ�� �Ҵ��ϱ� (���� ������ �ʻ�ȭ ID�� �Ҵ��� ���ڸ� ã�� �� ���� �� 10000ũ��� �Ҵ�)
        if (int.TryParse(lastSplit[0], out lastPortraitID))
        {
            charFolderNames = new string[lastPortraitID + 1];
            imageArray = new Sprite[lastPortraitID + 1, 100];
        }
        else
        {
            Debug.LogWarning($"�ʻ�ȭ ������ ������ �̸�, \"{charFolderNamesTemp[charFolderNamesTemp.Length - 1]}\"�� �ùٸ��� ���� ���� �̹Ƿ� �̹��� �迭, ���� �̸� �迭�� ũ�Ⱑ 10000��ŭ �Ҵ�Ǿ����ϴ�.");
            charFolderNames = new string[10000];
            imageArray = new Sprite[10000, 100];
        }


        //ĳ���� ���� �̸� ����� ���� �̸� �Ҵ��ϱ�
        for (int i = 0; i < charFolderNamesTemp.Length; i++)
        {
            //ĳ���� ID ���ϰ� �Ҵ��ϱ�
            string folderName = Path.GetFileNameWithoutExtension(charFolderNamesTemp[i]);
            string[] splitName = folderName.Split('-');

            int charPortraitID;

            if (int.TryParse(splitName[0], out charPortraitID))
            {
                charFolderNames[charPortraitID] = folderName;
            }
            else
            {
                Debug.LogWarning($"�ʻ�ȭ ������ �̸�,{folderName} �� �ùٸ��� ���� ���� �̹Ƿ�, �ش� ���� �̸��� �迭�� �Ҵ� �� �� �����ϴ�.");
            }
        }
    }

    void LoadImages()
    {
        for(int i = 0; i < charFolderNames.Length; i++)
        {
            if (charFolderNames[i] != string.Empty)
            {
                //TODO: charFolderNameLookUps���� ���� �̸��� �����ͼ� mainFolderPath�� ���� �Ϸ��� imageArray�� �Ҵ� �Ϸ��� ĳ������ ���� ��θ� ���� ��,
                //���� �� �̹������� �����ͼ� imageArray�� imageArray[ĳ���� ��ȣ][ǥ�� ��ȣ]��ġ�� �Ҵ��ϴ� ��ũ��Ʈ �ۼ�
                string path = mainFolderPath + '\\' + charFolderNames[i];

                string[] ImagePaths = Directory.GetFiles(path, "*.png"); // ���� ���� ��� PNG ���� �̸� ��������
                string[] imageNames = new string[ImagePaths.Length];     // ���� ���� �̹��� �̸���

                //�Ҵ��ϱ�
                for (int o = 0; o < ImagePaths.Length; o++)
                {
                    int assignLocation = Int32.Parse(Path.GetFileNameWithoutExtension(ImagePaths[o]).Split('-')[0]);

                    //�̹��� �ε�
                    Texture2D texture = LoadTextureFromFile(ImagePaths[o]);
                    if(texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0, SpriteMeshType.FullRect, new Vector4(0, 0, texture.width, texture.height), false);
                        sprite.texture.filterMode = FilterMode.Point; // FilterMode�� None���� ����

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
        texture.LoadImage(fileData); // ���� �����ͷκ��� �ؽ�ó �ε�
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
