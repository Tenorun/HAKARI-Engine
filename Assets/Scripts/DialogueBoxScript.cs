using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class DialogueBoxScript : MonoBehaviour
{
    //�ν��Ͻ�
    public static DialogueBoxScript instance;

    //�޽��� �ڽ�, �ʻ�ȭ �ڽ�, �ʻ�ȭ, �̸� �ڽ�, �� ���� ǥ�� ������Ʈ
    //"Message Box"
    public GameObject messageBox;
    //"Portrait Box"
    public GameObject portraitBox;
    //"Portrait Image"
    public Image portrait;
    //"Name Box"
    public GameObject nameBox;
    //"Line End Indicator"
    public GameObject lineEndIndicator;

    // �޽���, �̸� �ؽ�Ʈ
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI nameText;

    //�Ŵ�����
    public GameObject gameManager;
    public GameObject sfxManager;
    public GameObject portraitImageManager;

    //��� �ڽ� ��� ����
    public enum processMode
    {
        printText,
        getCommandType,
        getCommandVariable,
        lineEnd,
        sleep,
        disabled
    }

    //���� ��� �ڽ� ���
    public processMode currentMode = processMode.disabled;

    //�⺻ �޽��� ���� �ð�
    const float DEFAULT_DELAY_TIME = 0.05f;
    //�޽��� ���� �ð�
    float delayTime = DEFAULT_DELAY_TIME;
    //�޽��� ���� �ð� ī��Ʈ
    float delayCount = 0f;

    //�ֹ����� �޽���
    public string inputMessage;
    //ǥ�ñ⿡ ǥ�õǴ� �޽���
    public string displayedString;

    //�޽��� ������ �ι��� �̸�   (�������: �̸� ����, ��Ȱ��ȭ)
    string messengerName = string.Empty;

    //��ȭ ȿ���� ��Ʈ ����
    bool isVoiceSFXMuted = false;
    //��ȭ ȿ������ ���� ID
    int voiceSoundID = 1;

    // �ʻ�ȭ ������ Ȱ��ȭ ���¸� �����ϱ� ���� ����
    bool isPortraitBoxActive = false;

    void ResetDialogueBox()
    {
        //���� �ʱ�ȭ
        currentMode = processMode.disabled;

        displayedString = string.Empty;
        inputMessage = string.Empty;
        delayTime = DEFAULT_DELAY_TIME;
        delayCount = 0f;
        messengerName = string.Empty;
        isVoiceSFXMuted = false;
        voiceSoundID = 1;

        scanPosition = 0;
        scannedChar = '\0';
        commandType = string.Empty;
        commandVariable = string.Empty;

        sleepTime = 0f;
        sleepCount = 0f;

        //ǥ�� �ʱ�ȭ
        messageBox.SetActive(false);
        nameBox.SetActive(false);
        lineEndIndicator.SetActive(false);

        // �ʻ�ȭ ������ Ȱ��ȭ ���� ����
        portraitBox.SetActive(isPortraitBoxActive);
    }

    Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // ���� �����ͷκ��� �ؽ�ó �ε�
        return texture;
    }

    //��� ǥ�� ������Ʈ

    float endIndicatorVar = 0f;
    void UpdateDialogueSpaceDisplay()
    {
        switch (currentMode)
        {
            default:
                //�޽��� ����
                processMessage();

                //�޽��� ǥ�� ������Ʈ
                messageBox.SetActive(true);
                messageText.text = displayedString;

                //�̸� ǥ�� ������Ʈ
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

                //HAKARI ����,(�¿�� �����̴� �ൿ���� HAKARI �����̸�, endIndicator��� ��� ��ü�� �ٸ��� ���鶧�� �� �� �ִ�.)
                endIndicatorVar += Time.deltaTime;

                lineEndIndicator.transform.localPosition = new Vector2((-2.5f * Mathf.Cos(endIndicatorVar * 10)) + 154.5f, lineEndIndicator.transform.localPosition.y);
                if (endIndicatorVar >= Mathf.PI * 2)
                {
                    endIndicatorVar = 0f;
                }
                //HAKARI ����

                if (Input.GetButton("Submit"))
                {
                    if (scanPosition < inputMessage.Length)
                    {
                        currentMode = processMode.printText;
                        displayedString = string.Empty;
                        lineEndIndicator.SetActive(false);
                        endIndicatorVar = 0f;
                    }
                    else
                    {
                        bool isPfpActive = portraitBox.activeSelf;
                        ResetDialogueBox();
                        portraitBox.SetActive(isPfpActive);
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

    //����� ��� �Է� �ޱ�
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
        //���� ��ĵ�ϱ�
        if (scanPosition < inputMessage.Length)
        {
            scannedChar = inputMessage[scanPosition++];
        }
        else
        {
            currentMode = processMode.lineEnd;
        }

        switch (currentMode)
        {
            //�޽��� ��� ���
            case processMode.printText:

                //���� ��ĵ�� ���ڰ� ��ɾ��� ������ '#' �̶�� ��ɾ� �Է� ���� ��ȯ
                if (scannedChar == '#')
                {
                    currentMode = processMode.getCommandType;
                    processMessage();
                }
                //�ƴϸ� ǥ�� ���ڿ��� ��ĵ�� ���� �߰�
                else
                {
                    displayedString += scannedChar;

                    //��ȭ �Ҹ� ���
                    if (!isVoiceSFXMuted)
                    {
                        sfxManager.GetComponent<SFXManagerScript>().PlaySFX(voiceSoundID);
                    }

                    //���� ���� ���ڰ� ����� ������ �ǳʶٱ�
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

            //��ɾ� �Է� ���
            case processMode.getCommandType:

                //���� ��ĵ�� ���ڰ� ��� ������ ������ '(' �̶�� ��� ���� �Է� ���� ��ȯ
                if (scannedChar == '(')
                {
                    currentMode = processMode.getCommandVariable;
                }
                //�ƴϸ� ��ɾ� Ÿ�Կ� ��ĵ�� ���� �߰�
                else
                {
                    commandType += scannedChar;
                }
                processMessage();
                break;

            //��� ���� �Է� ���
            case processMode.getCommandVariable:

                //���� ��ĵ�� ���ڰ� ��� ������ ���� ')' �̶�� ��ɾ� ����
                if (scannedChar == ')')
                {
                    executeCommand(commandType, commandVariable);
                }
                //�ƴϸ� ��� ������ ��ĵ�� ���� �߰�
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

        switch (command)
        {
            //�ѹ��� ǥ���ϱ�
            case "displayAtOnce":
            case "dao":
                displayedString += variable;
                break;

            //��ȭ�� �̸� ����
            case "name":
                messengerName = variable;
                break;

            //��ȭ�� ���� ����
            case "portrait":
            case "pfp":
                //0 �̸� �ʱ�ȭ
                if (variable == "0")
                {
                    isPortraitBoxActive = false;
                    portraitBox.SetActive(false);
                    break;
                }

                //��ɾ� ������ �̹��� ID�� ��ȯ
                string[] splitVariable = variable.Split(',');
                int[] imageID = new int[2];

                //���� ���� �����Ͻ� ����ó��
                if (!int.TryParse(splitVariable[0], out imageID[0]))
                {
                    Debug.LogWarning($"�ʻ�ȭ ���� ����� ������ �ùٸ��� �ʽ��ϴ�: {variable}");
                    portraitBox.SetActive(false);
                    break;
                }
                if (!int.TryParse(splitVariable[1], out imageID[1]))
                {
                    Debug.LogWarning($"�ʻ�ȭ ���� ����� ������ �ùٸ��� �ʽ��ϴ�: {variable}");
                    portraitBox.SetActive(false);
                    break;
                }

                isPortraitBoxActive = true;
                portraitBox.SetActive(true);
                portrait.sprite = portraitImageManager.GetComponent<PortraitManager>().imageArray[imageID[0], imageID[1]];
                break;

            //1 ���� ǥ�� ������ �ð� ���� (��� ������ �ƹ��͵� �Է����� ������ �⺻��)
            case "delayTime":
            case "delaytime":
            case "dt":
                if (variable == string.Empty)
                {
                    delayTime = DEFAULT_DELAY_TIME;
                }
                else
                {
                    float targetTime;
                    if (float.TryParse(variable, out targetTime))
                    {
                        delayTime = targetTime;
                    }
                    else
                    {
                        delayTime = DEFAULT_DELAY_TIME;
                        Debug.LogWarning($"���ڰ� �ƴ� ��,\"{variable}\"(��)�� ������ �ð����� �����Ͽ� ������ �ð��� �⺻������ �����մϴ�.");
                    }
                }
                break;

            //variable�� ���� ����
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
                    Debug.LogWarning($"���ڰ� �ƴ� ��,\"{variable}\"(��)�� ���� �ð��� �����Ͽ� ���� ����� ���� �� �� �����ϴ�.");
                }
                break;

            //ȿ���� ���
            case "playSound":
            case "playsound":
            case "sound":
            case "sfx":
                int soundID;
                if (int.TryParse(variable, out soundID))
                {
                    sfxManager.GetComponent<SFXManagerScript>().PlaySFX(soundID);
                }
                else
                {
                    Debug.LogWarning($"���ڰ� �ƴ� ��,\"{variable}\"(��)�� SoundID�� �����Ͽ� ȿ������ ��� �� �� �����ϴ�.");
                }
                break;

            //��ȭ ȿ���� ���Ұ�
            case "mute":
                isVoiceSFXMuted = true;
                break;

            //��ȭ ȿ���� ���Ұ� ����
            case "unmute":
                isVoiceSFXMuted = false;
                break;

            //��ȭ �� �� ������
            case "lineEnd":
            case "lineend":
            case "endl":
                currentMode = processMode.lineEnd;
                clearCommand();
                isDefaultEndExecution = false;
                break;

            //��ȭ ���� ����
            case "shutDown":
            case "shutdown":
            case "sd":
                ResetDialogueBox();
                isDefaultEndExecution = false;
                break;

            //���� ó��
            default:
                Debug.LogWarning($"{command}(��)�� ���� ����Դϴ�.");
                break;
        }

        //��ɾ� ���� ������ �⺻
        if (isDefaultEndExecution)
        {
            currentMode = processMode.printText;
            clearCommand();
        }
    }

    //Ŀ�ǵ� �ʱ�ȭ
    void clearCommand()
    {
        commandType = string.Empty;
        commandVariable = string.Empty;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //�Ŵ��� ������Ʈ �Ҵ�
        gameManager = GameObject.Find("Game Manager");
        sfxManager = gameManager.transform.Find("SFX Manager").gameObject;
        portraitImageManager = gameManager.transform.Find("Portrait Image Manager").gameObject;

        //�ڽ� ������Ʈ(��ȭâ ���� ���) �Ҵ�
        //�ڽ� 1����
        messageBox = transform.Find("Message Box").gameObject;
        portraitBox = transform.Find("Portrait Box").gameObject;

        //�ڽ� 2����
        portrait = portraitBox.transform.Find("Portrait Image").GetComponent<Image>();
        nameBox = messageBox.transform.Find("Name Box").gameObject;
        lineEndIndicator = messageBox.transform.Find("Line End Indicator").gameObject;
        messageText = messageBox.transform.Find("Message Text").GetComponent<TextMeshProUGUI>();

        //�ڽ� 3����
        nameText = nameBox.transform.Find("Name Text").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        portrait.GetComponent<Image>();
        ResetDialogueBox();
    }

    //���� �ð�
    float sleepTime = 0f;
    float sleepCount = 0f;

    public bool testTrigger = false;

    void Update()
    {
        //�׽�Ʈ��
        if (testTrigger)
        {
            testTrigger = false;
            GetInputMessage("#pfp(0,1)#name(����)�ȳ��ϼ���, ���� ������AU ���� ���ΰ��� �ð��ִ� ��� ����, �����Դϴ�.#endl()" +
                "����, ���� ���� �ൿ���� ���� ū ���ظ� ��ġ�� �Ǹ��� �帰 ��ϴ�, #slp(0.2)ģ�� �е鲲 �˼��մϴ�. #slp(1)���ݺ��ʹ�");
        }

        if (currentMode != processMode.sleep)
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
            if (sleepCount >= sleepTime)
            {
                sleepTime = 0f;
                sleepCount = 0f;
                currentMode = processMode.printText;
            }
        }
    }
}
