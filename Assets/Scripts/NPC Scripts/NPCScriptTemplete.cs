using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScriptTemplete : MonoBehaviour
{
    private Collider2D npcCollider;

    int sequenceNum = 0;
    bool progressSequence = true;

    public enum processMode
    {
        idle,
        say,
        other
    }

    public processMode currentMode;
    


    // Start is called before the first frame update
    void Start()
    {
        npcCollider = GetComponent<Collider2D>();

        currentMode = processMode.idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            //������
            case processMode.idle:
                //�÷��̾� ��ȣ�ۿ� Ȯ��
                if (PlayerControl.instance.rayHitObject == this.gameObject && PlayerControl.instance.isSubmitDown)
                {
                    PlayerControl.instance.isSubmitDown = false;
                    ExecuteLocalizedAction();
                }
                break;
            //��ȭ���� ����
            case processMode.say:
                //��ȭ �� ���� Ȯ��
                if(DialogueBoxScript.instance.currentMode == DialogueBoxScript.processMode.disabled)
                {
                    PlayerControl.instance.LockPlayerControl = false;
                    if (progressSequence)
                    {
                        ExecuteLocalizedAction();
                    }
                    else
                    {
                        progressSequence = true;
                        currentMode = processMode.idle;
                    }
                }
                break;
        }
    }

    //���� Ȯ���ϰ� ���� �� �´� �ൿ�� �����ϴ� �ڵ�.
    void ExecuteLocalizedAction()
    {
        switch (GameManagerScript.instance.languageVal)
        {
            case 0:
                EnglishAct();
                break;
            case 1:
                KoreanAct();
                break;
            default:
                Debug.LogError($"�� {GameManagerScript.instance.languageVal}�� ���� �ൿ�� �����ϴ�!");
                break;
        }
    }

    void EnglishAct()
    {
        PlayerControl.instance.LockPlayerControl = true;

        switch (sequenceNum)
        {
            case 0:
                Say("Hello.#endl()I'm Mari playing as lead role and Sunny's sister OMARI AU. #endl()First of all, to Sunny and his firends, because of my words and actions that caused critical damage, #slp(0.2)I apologize.#endl()from now on");
                break;
            default:
                Debug.LogWarning($"������ �� {sequenceNum}�� �ش��ϴ� �ൿ�� �����ϴ�!");
                break;
        }
    }

    void KoreanAct()
    {
        PlayerControl.instance.LockPlayerControl = true;

        switch (sequenceNum)
        {
            case 0:
                Say("#pfp(0,5)#name(����)�ȳ��ϼ���.#endl()����, ������ AU���� ���ΰ��� �ð��ִ� ��� ����, �����Դϴ�.#endl()����, ���� ����, �ൿ���� ���� ū ���ظ� ��ġ��, �Ǹ��� �帰 ��ϴ�, ģ���е鲲, #slp(0.2)�˼��մϴ�.#endl()���ݺ��ʹ�");
                sequenceNum++;
                break;
            case 1:
                EndSequence();
                Say("Hello World!");
                sequenceNum = 0;
                break;
            default:
                Debug.LogWarning($"������ �� {sequenceNum}�� �ش��ϴ� �ൿ�� �����ϴ�!");
                break;
        }
    }

    void EndSequence()
    {
        progressSequence = false;
    }

    void Say(string dialogueLine)
    {
        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }
}
