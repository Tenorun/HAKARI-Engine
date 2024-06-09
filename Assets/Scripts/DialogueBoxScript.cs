using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class DialogueBoxScript : MonoBehaviour
{

    //메시지 박스, 초상화 박스, 초상화, 이름 박스, 줄 끝남 표시 오브젝트
    public GameObject messageBox;
    public GameObject portraitBox;
    public Image portrait;
    public GameObject nameBox;
    public GameObject lineEndIndicator;

    // 메시지, 이름 텍스트
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI nameText;


    //대사 박스 모드 종류
    public enum processMode
    {
        printText,      
        getCommandType,
        getCommandVariable,
        lineEnd,
        sleep,
        disabled
    }

    //현재 대사 박스 모드
    public processMode currentMode = processMode.disabled;

    //기본 메시지 지연 시간
    const float DEFAULT_DELAY_TIME = 0.05f;
    //메시지 지연 시간
    float delayTime = DEFAULT_DELAY_TIME;
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



    //사운드 매니저 오브젝트
    public GameObject SoundManager;
    //대화 효과음 뮤트 여부
    bool isVoiceSFXMuted = false;
    //대화 효과음의 사운드 ID
    int voiceSoundID = 1;

    void ResetDialogueBox()
    {
        //변수 초기화
        currentMode = processMode.disabled;

        displayedString = string.Empty;
        inputMessage = string.Empty;
        delayTime = DEFAULT_DELAY_TIME;
        delayCount = 0f;
        portraitID = 0;
        messengerName = string.Empty;
        isVoiceSFXMuted = false;
        voiceSoundID = 1;
        

        scanPosition = 0;
        scannedChar = '\0';
        commandType = string.Empty;
        commandVariable = string.Empty;

        sleepTime = 0f;
        sleepCount = 0f;

        //표시 초기화
        messageBox.SetActive(false);
        portraitBox.SetActive(false);
        nameBox.SetActive(false);
        lineEndIndicator.SetActive(false);
    }

    Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // 파일 데이터로부터 텍스처 로드
        return texture;
    }

    //대사 표시 업데이트

    float endIndicatorVar = 0f;
    void UpdateDialogueSpaceDisplay()
    {
        switch (currentMode)
        {
            default:
                //메시지 진행
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
                if (messengerName != string.Empty)
                {
                    nameBox.SetActive(true);
                    nameText.text = messengerName;
                }
                else
                {
                    nameBox.SetActive(false);
                }
                break;

            case processMode.lineEnd:
                lineEndIndicator.SetActive(true);

                //HAKARI 전용,(좌우로 움직이는 행동만이 HAKARI 전용이며, endIndicator라는 기능 자체는 다른거 만들때도 쓸 수 있다.)
                endIndicatorVar += Time.deltaTime;

                lineEndIndicator.transform.localPosition = new Vector2((-2.5f * Mathf.Cos(endIndicatorVar * 10)) + 154.5f , lineEndIndicator.transform.localPosition.y);
                if(endIndicatorVar >= Mathf.PI * 2)
                {
                    endIndicatorVar = 0f;
                }
                //HAKARI 전용

                if (Input.GetButton("Submit"))
                {
                    if(scanPosition < inputMessage.Length)
                    {
                        currentMode = processMode.printText;
                        displayedString = string.Empty;
                        lineEndIndicator.SetActive(false);
                        endIndicatorVar = 0f;
                    }
                    else
                    {
                        ResetDialogueBox();
                    }
                }
                break;

            case processMode.disabled:

                messageBox.SetActive(false);
                portraitBox.SetActive(false);
                nameBox.SetActive(false);
                break;
        }
    }


    //출력할 대사 입력 받기
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

                    //대화 소리 재생
                    if (!isVoiceSFXMuted)
                    {
                        SoundManager.GetComponent<SFXManagerScript>().PlaySFX(voiceSoundID);
                    }

                    //만약 다음 문자가 띄어쓰기면 딜레이 건너뛰기
                    if (scanPosition < inputMessage.Length)
                    {
                        if (inputMessage[scanPosition] == ' ')
                        {
                            scanPosition++;
                            displayedString += ' ';
                        }
                    }
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
        bool isDefaultEndExecution = true;

        switch(command)
        {
            //한번에 표시하기
            case "displayAtOnce":
            case "dao":
                displayedString += variable;
                break;
            
            //대화자 이름 설정
            case "name":
                messengerName = variable;
                break;

            //대화자 사진 설정
            case "portrait":
            case "pfp":
                
                break;
            
            //1 글자 표시 딜레이 시간 설정 (명령 변수에 아무것도 입력하지 않을시 기본값)
            case "delayTime":
            case "delaytime":
            case "dt":
                if(variable == string.Empty)
                {
                    delayTime = DEFAULT_DELAY_TIME;
                }
                else
                {
                    float targetTime;
                    if(float.TryParse(variable, out targetTime))
                    {
                        delayTime = targetTime;
                    }
                    else
                    {
                        delayTime = DEFAULT_DELAY_TIME;
                        Debug.LogWarning($"숫자가 아닌 값,\"{variable}\"(을)를 딜레이 시간으로 설정하여 딜레이 시간을 기본값으로 설정합니다.");
                    }
                }
                break;

            //variable초 동안 정지
            case "sleep":
            case "freeze":
            case "slp":
            case "frz":
                float targetFreeze;
                if (float.TryParse(variable, out targetFreeze))
                {
                    currentMode = processMode.sleep;
                    delayCount = 0f;
                    sleepTime = targetFreeze;

                    clearCommand();
                    isDefaultEndExecution = false;
                }
                else
                {
                    Debug.LogWarning($"숫자가 아닌 값,\"{variable}\"(을)를 정지 시간로 설정하여 정지 명령을 실행 할 수 없습니다.");
                }
                break;
            
            //효과음 재생
            case "playSound":
            case "playsound":
            case "sound":
            case "sfx":
                int soundID;
                if (int.TryParse(variable, out soundID))
                {
                    SoundManager.GetComponent<SFXManagerScript>().PlaySFX(soundID);
                }
                else
                {
                    Debug.LogWarning($"숫자가 아닌 값,\"{variable}\"(을)를 SoundID로 설정하여 효과음을 재생 할 수 없습니다.");
                }
                break;

            //대화 효과음 음소거
            case "mute":
                isVoiceSFXMuted = true;
                break;

            //대화 효과음 음소거 해제
            case "unmute":
                isVoiceSFXMuted = false;
                break;


            //대화 한 줄 끝내기
            case "lineEnd":
            case "lineend":
            case "endl":
                currentMode = processMode.lineEnd;
                clearCommand();
                isDefaultEndExecution = false;
                break;

            //대화 강제 종료
            case "shutDown":
            case "shutdown":
            case "sd":
                ResetDialogueBox();
                isDefaultEndExecution = false;
                break;

            //예외 처리
            default:
                Debug.LogWarning($"{command}(은)는 없는 명령입니다.");
                break;

        }

        //명령어 실행 끝내기 기본
        if (isDefaultEndExecution)
        {
            currentMode = processMode.printText;
            clearCommand();
        }
    }

    //커맨드 초기화
    void clearCommand()
    {
        commandType = string.Empty;
        commandVariable = string.Empty;
    }

    void Start()
    {
        portrait.GetComponent<Image>();
        ResetDialogueBox();
    }

    //정지 시간
    float sleepTime = 0f;
    float sleepCount = 0f;

    public bool testTrigger = false;

    void Update()
    {
        //테스트용
        if(testTrigger)
        {
            testTrigger = false;
            GetInputMessage("#pfp(1)#name(마리)안녕하세요, 저는 오마리AU 에서 주인공을 맡고있는 #pfp(0)마리입니다.#endl()" +
                "먼저, 저의 말과 행동으로 인해 큰 피해를 끼치고 실망을 드린 써니님, #slp(0.2)친구 분들께 죄송합니다. #slp(2)지금부터는");
        }

        if(currentMode != processMode.sleep)
        {
            delayCount += Time.deltaTime;
            if (delayCount >= delayTime || currentMode == processMode.lineEnd)
            {
                delayCount = 0f;
                UpdateDialogueSpaceDisplay();
            }
        }
        else
        {
            sleepCount += Time.deltaTime;
            if(sleepCount >= sleepTime)
            {
                sleepTime = 0f;
                sleepCount = 0f;
                currentMode = processMode.printText;
            }
        }
    }
}
