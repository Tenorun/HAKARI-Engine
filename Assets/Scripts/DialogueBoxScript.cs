using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxScript : MonoBehaviour
{

    //�޽��� �ڽ�, �ʻ�ȭ �ڽ�, �ʻ�ȭ , �̸� �ڽ� ������Ʈ
    public GameObject messageBox;
    public GameObject portraitBox;
    public GameObject portrait;
    public GameObject nameBox;

    // �޽���, �̸� �ؽ�Ʈ
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI nameText;

    //�ʻ�ȭ �̹���
    public string relativeFolderPath;   //������ ������ ��� ���
    public Sprite[] imageArray;         //�̹����� ������ �迭

    //�޽��� �ڽ� Ȱ��ȭ ����
    bool isActive = false;
    //�޽��� ���� �ð�
    const float delayTime = 0.2f;
    //�޽��� ���� �ð� ī��Ʈ
    float delayCount = 0f;

    //�ֹ����� �޽���
    public string inputMessage;
    //ǥ�ñ⿡ ǥ�õǴ� �޽���
    public string displayedString;

    //�ʻ�ȭ ǥ�� ���� ID  (0: ��Ȱ��ȭ)
    int portraitID = 0;
    //�޽��� ������ �ι��� �̸�   (�������: �̸� ����, ��Ȱ��ȭ)
    string messengerName = string.Empty;


    



    void ResetDialogueBox()
    {
        //���� �ʱ�ȭ
        isActive = false;
        displayedString = string.Empty;
        inputMessage = string.Empty;
        delayCount = 0f;
        portraitID = 0;
        messengerName = string.Empty;


        scanPosition = 0;
        scannedChar = '\0';
        command = string.Empty;
        commandElement = string.Empty;
    }

    void UpdateDialogueSpaceDisplay()
    {
        if (isActive)
        {
            //�޽��� ǥ�� ������Ʈ
            messageBox.SetActive(true);
            messageText.text = displayedString;

            //�ʻ�ȭ ǥ�� ������Ʈ
            if (portraitID != 0)
            {
                portraitBox.SetActive(true);
                //TODO: �ʻ�ȭ �̹����� �ʻ�ȭ �ڽ��� �Ҵ� ��� ����
            }
            else
            {
                portraitBox.SetActive(false);
            }

            //�̸� ǥ�� ������Ʈ
            if(messengerName != string.Empty)
            {
                nameBox.SetActive(true);
                nameText.text = messengerName;
            }
            else
            {
                nameBox.SetActive(false);
            }
        }
        else
        {
            messageBox.SetActive(false);
            portraitBox.SetActive(false);
            nameBox.SetActive(false);
        }
    }

    public void GetInputMessage(string messageIn)
    {
        ResetDialogueBox();

        isActive = true;
        inputMessage = messageIn;

    }

    int scanPosition = 0;
    char scannedChar = '\0';

    string command;
    string commandElement;

    void processMessage()
    {
        if (isActive)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetDialogueBox();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDialogueSpaceDisplay();
    }
}
