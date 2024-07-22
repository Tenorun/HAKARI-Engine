using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScriptTemplete : MonoBehaviour
{
    private Collider2D npcCollider;

    //시퀀스 번호(기본값: 0)
    int sequenceNum = 0;
    //시퀀스 지속 여부(기본값: true)
    bool progressSequence = true;

    public enum processMode
    {
        idle,
        say,
        move
    }

    public processMode currentMode; //현재 모드

    private bool doIdleAct = true;  //idleAct 실행 여부

    public Transform[] targetPosition; // 이동할 목표 위치를 나타내는 Transform

    public float moveSpeed = 2.0f; // 이동 속도

    [SerializeField] bool isMoving = false; // 이동 중인지 여부

    private Coroutine moveCoroutine; // 이동 코루틴 참조 변수


    // Start is called before the first frame update
    void Start()
    {
        npcCollider = GetComponent<Collider2D>();
        currentMode = processMode.idle;
    }

    void Update()
    {
        switch (currentMode)
        {
            case processMode.idle:
                if (PlayerControl.instance.rayHitObject == this.gameObject && PlayerControl.instance.isSubmitDown)
                {
                    PlayerControl.instance.isSubmitDown = false;
                    ExecuteLocalizedAction();
                }
                else if (doIdleAct)
                {
                    idleAct();
                }
                break;
            case processMode.say:
                if (DialogueBoxScript.instance.currentMode == DialogueBoxScript.processMode.disabled)
                {
                    if (progressSequence)
                    {
                        ExecuteLocalizedAction();
                    }
                    else
                    {
                        PlayerControl.instance.LockPlayerControl = false;
                        progressSequence = true;
                        currentMode = processMode.idle;
                    }
                }
                break;
            case processMode.move:
                PlayerControl.instance.LockPlayerControl = true;
                if (!isMoving)
                {
                    if (progressSequence)
                    {
                        ExecuteLocalizedAction();
                    }
                    else
                    {
                        PlayerControl.instance.LockPlayerControl = false;
                        progressSequence = true;
                        currentMode = processMode.idle;
                    }
                }
                break;
        }
    }


    //기본상태의 행동(ex: 경로를 따라 움직임)
    [SerializeField] int idleSequenceNum = 0;            //대기 상태 시퀀스 번호
    [SerializeField] bool progressIdleSequence = true;   //시퀀스 넘어감 신호

    processMode idleCurrentMode = processMode.idle;
    void idleAct()
    {
        if (progressIdleSequence)
        {
            //시퀀스 진행
            switch (idleSequenceNum)
            {
                case 0:
                    idleSequenceNum++;
                    progressIdleSequence = false;
                    idleCurrentMode = processMode.move;
                    MoveToTarget(targetPosition[0]);
                    Debug.Log("나는!");
                    break;
                case 1:
                    idleSequenceNum = 0;
                    progressIdleSequence = false;
                    idleCurrentMode = processMode.move;
                    MoveToTarget(targetPosition[1]);
                    Debug.Log("섹시가이.");
                    break;
                default:
                    idleSequenceNum = 0;
                    Debug.LogWarning($"{idleSequenceNum}에 해당하는 대기 상태 행동이 없어 0으로 설정합니다.");
                    break;
            }
        }
        else
        {
            switch (idleCurrentMode)
            {
                case processMode.move:
                    if (!isMoving)
                    {
                        idleCurrentMode = processMode.idle;
                        progressIdleSequence = true;
                    }
                    break;
            }
        }
    }

    //언어별 행동 실행
    void ExecuteLocalizedAction()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            isMoving = false;
            moveCoroutine = null;
        }

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

    //영어 행동
    void EnglishAct()
    {
        doIdleAct = false;
        PlayerControl.instance.LockPlayerControl = true;
        PlayerControl.instance.anim.SetBool("is Moving", false);

        switch (sequenceNum)
        {
            case 0:
                Say("Hello.#endl()" +
                    "I'm Mari playing as lead role and Sunny's sister OMARI AU. #endl()" +
                    "First of all, to Sunny and his firends, because of my words and actions that caused critical damage, I apologize.#endl()" +
                    "from now on");
                sequenceNum++;
                break;
            case 1:
                Teleport(targetPosition[0]);
                sequenceNum++;
                break;
            case 2:
                MoveToTarget(targetPosition[1]);
                sequenceNum = 0;
                CutSequence();
                break;
            default:
                Debug.LogWarning($"시퀀스 값 {sequenceNum}에 해당하는 행동이 없습니다!");
                break;
        }
    }

    void KoreanAct()
    {
        doIdleAct = false;
        PlayerControl.instance.LockPlayerControl = true;
        PlayerControl.instance.anim.SetBool("is Moving", false);

        switch (sequenceNum)
        {
            case 0:
                Say("#pfp(0,5)#name(마리)안녕하세요.#endl()" +
                    "저는, 오마리 AU에서 주인공을 맡고있는 써니 누나, 마리입니다.#endl()" +
                    "#pfp(0,2)먼저, 저의 말과, 행동으로 인해 큰 피해를 끼치고, 실망을 드린 써니님, 친구분들께, 죄송합니다.#endl()" +
                    "지금부터는");
                sequenceNum++;
                break;
            case 1:
                Teleport(targetPosition[0]);
                sequenceNum++;
                break;
            case 2:
                MoveToTarget(targetPosition[1]);
                sequenceNum = 0;
                CutSequence();
                break;
            default:
                Debug.LogWarning($"시퀀스 값 {sequenceNum}에 해당하는 행동이 없습니다!");
                break;
        }
    }


    //시퀀스 끊기
    void CutSequence()
    {
        doIdleAct = true;
        progressSequence = false;
    }

    //말하기
    void Say(string dialogueLine)
    {
        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }

    //타겟으로 텔레포트
    void Teleport(Transform target)
    {
        transform.position = target.position;
        Debug.Log($"NPC has been teleported to {target.position}");
    }

    //타겟으로 움직임
    void MoveToTarget(Transform target)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        isMoving = true;
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine(target));
    }

    //타겟으로 움직이는 과정
    IEnumerator MoveToTargetCoroutine(Transform target)
    {
        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target.position;
        isMoving = false;
        moveCoroutine = null;
        Debug.Log("NPC has reached the target position");
    }
}
