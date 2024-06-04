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


    //��� �ڽ� ��� ����
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

    //���� ��� �ڽ� ���
    public processMode currentMode = processMode.disabled;

    //�޽��� ���� �ð�
    float delayTime = 0.075f;
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
            //�޽��� ��� ���
            case processMode.printText:

                //���� ��ĵ�� ���ڰ� ��ɾ��� ������ '#' �̶�� ��ɾ� �Է� ���� ��ȯ
                if(scannedChar == '#')
                {
                    currentMode = processMode.getCommandType;
                    processMessage();
                }
                //�ƴϸ� ǥ�� ���ڿ��� ��ĵ�� ���� �߰�
                else
                {
                    displayedString += scannedChar;
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
            GetInputMessage("#name(����)�ȳ�! ���! #dao(������ �����ؾ���?)#dao(������ �����ؾ���?)#dao(������ �����ؾ���?)#dao(������ �����ؾ���?)#dao(������ �����ؾ���?)#dao(������ �����ؾ���?)#dao(������ �����ؾ���?)#dao(������ �����ؾ���?)");
        }

        delayCount += Time.deltaTime;
        if(delayCount >= delayTime)
        {
            delayCount = 0f;
            UpdateDialogueSpaceDisplay();
        }
    }
}
