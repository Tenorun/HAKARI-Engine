using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxScript : MonoBehaviour
{

    //메시지 박스, 초상화 박스, 초상화 , 이름 박스 오브젝트
    public GameObject messageBox;
    public GameObject portraitBox;
    public GameObject portrait;
    public GameObject nameBox;

    // 메시지, 이름 텍스트
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI nameText;

    //초상화 이미지
    public string relativeFolderPath;   //데이터 폴더의 상대 경로
    public Sprite[] imageArray;         //이미지를 저장할 배열

    //메시지 박스 활성화 여부
    bool isActive = false;
    //메시지 지연 시간
    const float delayTime = 0.2f;
    //메시지 지연 시간 카운트
    float delayCount = 0f;

    //주문받은 메시지
    public string inputMessage;
    //표시기에 표시되는 메시지
    public string displayedString;

    //초상화 표시 사진 ID  (0: 비활성화)
    int portraitID = 0;
    //메시지 보내는 인물의 이름   (비어있음: 이름 없음, 비활성화)
    string messengerName = string.Empty;


    



    void ResetDialogueBox()
    {
        //변수 초기화
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
            //메시지 표시 업데이트
            messageBox.SetActive(true);
            messageText.text = displayedString;

            //초상화 표시 업데이트
            if (portraitID != 0)
            {
                portraitBox.SetActive(true);
                //TODO: 초상화 이미지를 초상화 박스에 할당 기능 구현
            }
            else
            {
                portraitBox.SetActive(false);
            }

            //이름 표시 업데이트
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
