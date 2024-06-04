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


    //대사 박스 모드 종류
    public enum processMode
    {
        printText,      
        getCommandType,
        getCommandVariable,
        executeCommand,
        lineEnd,
        dialogueEnd,
        disabled
    }

    //현재 대사 박스 모드
    public processMode currentMode = processMode.disabled;

    //메시지 지연 시간
    float delayTime = 0.075f;
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
        currentMode = processMode.disabled;

        displayedString = string.Empty;
        inputMessage = string.Empty;
        delayCount = 0f;
        portraitID = 0;
        messengerName = string.Empty;


        scanPosition = 0;
        scannedChar = '\0';
        commandType = string.Empty;
        commandVariable = string.Empty;

        messageBox.SetActive(false);
        portraitBox.SetActive(false);
        nameBox.SetActive(false);
    }

    void UpdateDialogueSpaceDisplay()
    {
        if (currentMode != processMode.disabled)
        {
            processMessage();

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

        currentMode = processMode.printText;
        inputMessage = messageIn;

    }

    int scanPosition = 0;
    char scannedChar = '\0';

    string commandType;
    string commandVariable;

    void processMessage()
    {
        //글자 스캔하기
        if(scanPosition < inputMessage.Length)
        {
            scannedChar = inputMessage[scanPosition++];
        }
        else
        {
            currentMode = processMode.lineEnd;
        }


        switch (currentMode)
        {
            //메시지 출력 모드
            case processMode.printText:

                //만약 스캔된 글자가 명령어의 시작인 '#' 이라면 명령어 입력 모드로 전환
                if(scannedChar == '#')
                {
                    currentMode = processMode.getCommandType;
                    processMessage();
                }
                //아니면 표시 문자열에 스캔한 문자 추가
                else
                {
                    displayedString += scannedChar;
                }
                break;

            //명령어 입력 모드
            case processMode.getCommandType:

                //만약 스캔된 글자가 명령 변수의 시작인 '(' 이라면 명령 변수 입력 모드로 전환
                if (scannedChar == '(')
                {
                    currentMode = processMode.getCommandVariable;
                }
                //아니면 명령어 타입에 스캔한 글자 추가
                else
                {
                    commandType += scannedChar;
                }
                processMessage();
                break;

            //명령 변수 입력 모드
            case processMode.getCommandVariable:

                //만약 스캔된 글자가 명령 변수의 끝인 ')' 이라면 명령어 실행
                if (scannedChar == ')')
                {
                    executeCommand(commandType, commandVariable);
                }
                //아니면 명령 변수에 스캔한 글자 추가
                else
                {
                    commandVariable += scannedChar;
                    processMessage();
                }
                break;
        }
    }

    void executeCommand(string command, string variable)
    {
        switch(command)
        {
            case "test":
                Debug.Log("test");
                currentMode = processMode.printText;
                clearCommand();
                break;
            case "displayAtOnce":
            case "dao":
                displayedString += variable;
                currentMode = processMode.printText;
                clearCommand();
                break;
            case "setName":
            case "name":
                messengerName = variable;
                currentMode = processMode.printText;
                clearCommand();
                break;
        }
    }

    void clearCommand()
    {
        commandType = string.Empty;
        commandVariable = string.Empty;
    }

    void Start()
    {
        ResetDialogueBox();
    }

    public bool testTrigger = false;

    void Update()
    {
        if(testTrigger)
        {
            testTrigger = false;
            GetInputMessage("#name(마리)안녕! 써니! #dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)#dao(누나랑 연습해야지?)");
        }

        delayCount += Time.deltaTime;
        if(delayCount >= delayTime)
        {
            delayCount = 0f;
            UpdateDialogueSpaceDisplay();
        }
    }
}
