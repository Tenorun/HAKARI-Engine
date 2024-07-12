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
            //대기상태
            case processMode.idle:
                //플레이어 상호작용 확인
                if (PlayerControl.instance.rayHitObject == this.gameObject && PlayerControl.instance.isSubmitDown)
                {
                    PlayerControl.instance.isSubmitDown = false;
                    ExecuteLocalizedAction();
                }
                break;
            //대화중인 상태
            case processMode.say:
                //대화 끝 여부 확인
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

    //언어값을 확인하고 현재 언어에 맞는 행동을 실행하는 코드.
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
                Debug.LogError($"언어값 {GameManagerScript.instance.languageVal}에 대한 행동이 없습니다!");
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
                Debug.LogWarning($"시퀀스 값 {sequenceNum}에 해당하는 행동이 없습니다!");
                break;
        }
    }

    void KoreanAct()
    {
        PlayerControl.instance.LockPlayerControl = true;

        switch (sequenceNum)
        {
            case 0:
                Say("#pfp(0,5)#name(마리)안녕하세요.#endl()저는, 오마리 AU에서 주인공을 맡고있는 써니 누나, 마리입니다.#endl()먼저, 저의 말과, 행동으로 인해 큰 피해를 끼치고, 실망을 드린 써니님, 친구분들께, #slp(0.2)죄송합니다.#endl()지금부터는");
                sequenceNum++;
                break;
            case 1:
                EndSequence();
                Say("Hello World!");
                sequenceNum = 0;
                break;
            default:
                Debug.LogWarning($"시퀀스 값 {sequenceNum}에 해당하는 행동이 없습니다!");
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
