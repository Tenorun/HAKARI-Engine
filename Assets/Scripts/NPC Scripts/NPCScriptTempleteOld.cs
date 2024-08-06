using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCScriptTemplateOld : MonoBehaviour
{
    private Collider2D npcCollider;
    public Animator anim;

    public float stareAngle = -90f;
    public bool isWalking = false;

    public bool handleDefaultWalkAnim = true;


    // 시퀀스 번호(기본값: 0)
    int sequenceNum = 0;
    // 시퀀스 지속 여부(기본값: true)
    bool progressSequence = true;

    public enum processMode
    {
        idle,
        say,
        move,
        sleep
    }

    public processMode currentMode; // 현재 모드

    private bool doIdleAct = true;  // idleAct 실행 여부

    public Transform[] targetPosition; // 이동할 목표 위치를 나타내는 Transform

    private float moveSpeed = 3.0f; // 이동 속도

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
            case processMode.sleep:
                // Do nothing while sleeping
                break;
        }



        //기본 걷기 애니메이션 업데이트
        if (handleDefaultWalkAnim)
        {
            anim.SetBool("is Moving", isWalking);
            anim.SetFloat("Stare Angle", stareAngle + 0.000001f);
        }
    }


    // 기본상태의 행동(ex: 경로를 따라 움직임)
    [SerializeField] int idleSequenceNum = 0;            // 대기 상태 시퀀스 번호
    [SerializeField] bool progressIdleSequence = true;   // 시퀀스 넘어감 신호

    processMode idleCurrentMode = processMode.idle;
    void idleAct()
    {
        if (progressIdleSequence)
        {
            progressIdleSequence = false;
            // 시퀀스 진행
            switch (idleSequenceNum)
            {
                case 0:
                    idleSequenceNum++;
                    idleCurrentMode = processMode.move;
                    WalkToTarget(targetPosition[0], false);
                    break;
                case 1:
                    idleSequenceNum++;
                    idleCurrentMode = processMode.move;
                    WalkToTarget(targetPosition[1], false);
                    break;
                case 2:
                    idleSequenceNum++;
                    idleCurrentMode = processMode.move;
                    WalkToTarget(targetPosition[2], false);
                    break;
                case 3:
                    idleSequenceNum++;
                    isWalking = false;
                    StartCoroutine(Sleep(0.2f, true));
                    break;
                case 4:
                    idleSequenceNum++;
                    stareAngle = -90f;
                    StartCoroutine(Sleep(0.8f, true));
                    break;
                case 5:
                    idleSequenceNum = 0;
                    idleCurrentMode = processMode.move;
                    WalkToTarget(targetPosition[3], false);
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
                case processMode.sleep:
                    //do nothing while sleeping
                    break;
            }
        }
    }

    // 언어별 행동 실행
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

    // 영어 행동
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
                    "First of all, to Sunny and his friends, because of my words and actions that caused critical damage, I apologize.#endl()" +
                    "from now on", true);
                sequenceNum++;
                break;
            case 1:
                Teleport(targetPosition[0]);
                sequenceNum++;
                break;
            case 2:
                WalkToTarget(targetPosition[1], true);
                sequenceNum++;
                break;
            case 3:
                WalkToTarget(targetPosition[2], true);
                sequenceNum++;
                break;
            case 4:
                WalkToTarget(targetPosition[3], true);
                sequenceNum++;
                break;
            default:
                Debug.LogWarning($"시퀀스 값 {sequenceNum}에 해당하는 행동이 없습니다!");
                sequenceNum = 0;
                Debug.LogWarning($"시퀀스 값이 0으로 돌아갑니다.");
                break;
        }
    }

    //한국어 행동
    void KoreanAct()
    {
        doIdleAct = false;
        PlayerControl.instance.LockPlayerControl = true;
        PlayerControl.instance.anim.SetBool("is Moving", false);

        switch (sequenceNum)
        {
            case 0:
                Say("#pfp(1,1)#name(써니)안녕하세요.#endl()" +
                    "저는, 오모리에서 주인공을 맡고있는 마리 동생, 써니 입니다.#endl()" +
                    "#pfp(1,2)먼저, 저의 말과, 행동으로 인해 큰 피해를 끼치고, 실망을 드린 마리님, 친구분들께, 죄송합니다.", true);
                sequenceNum++;
                break;
            case 1:
                Say("#pfp(1,1)#name(써니)지금부터는", true);
                sequenceNum++;
                break;
            case 2:
                WalkToTarget(targetPosition[1], true);
                sequenceNum++;
                break;
            case 3:
                stareAngle = -90f;
                isWalking = false;
                sequenceNum++;
                break;
            case 4:
                StartCoroutine(Sleep(3, false)); // Sleep for 3 seconds
                sequenceNum = 0;
                CutSequence();
                break;
            default:
                Debug.LogWarning($"시퀀스 값 {sequenceNum}에 해당하는 행동이 없습니다!");
                sequenceNum = 0;
                Debug.LogWarning($"시퀀스 값이 0으로 돌아갑니다.");
                break;
        }
    }

    // 시퀀스 끊기
    void CutSequence()
    {
        doIdleAct = true;
        progressSequence = false;
    }

    // 말하기
    void Say(string dialogueLine, bool stopAndStare)
    {
        if (stopAndStare)
        {
            isWalking = false;
            Vector2 directionToTarget = PlayerControl.instance.gameObject.transform.position - transform.position; // 타겟 방향 벡터 계산
            stareAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // 각도 계산
        }

        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }

    // 타겟으로 텔레포트
    void Teleport(Transform target)
    {
        transform.position = target.position;
    }

    // 타겟으로 움직임
    void MoveToTarget(Transform target, bool lockPlayerUntilFinish)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        isMoving = true;

        if (lockPlayerUntilFinish)
        {
            currentMode = processMode.move;
        }
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine(target));
    }

    void WalkToTarget(Transform target, bool lockPlayerUntilFinish)
    {
        isWalking = true;

        // 이 오브젝트에서 target를 향하는 각도를 구해서 stareAngle에 적용
        Vector2 directionToTarget = target.position - transform.position; // 타겟 방향 벡터 계산
        stareAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // 각도 계산

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            isWalking = false;
        }
        isMoving = true;

        if (lockPlayerUntilFinish)
        {
            currentMode = processMode.move;
        }
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine(target));
    }


    // 타겟으로 움직이는 과정
    IEnumerator MoveToTargetCoroutine(Transform target)
    {
        while (Vector2.Distance(transform.position, target.position) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target.position;
        isMoving = false;
        moveCoroutine = null;
    }

    // 잠자기
    IEnumerator Sleep(float seconds, bool isIdleMove)
    {
        //슬립 걸기
        if (isIdleMove) idleCurrentMode = processMode.sleep;
        else currentMode = processMode.sleep;

        //잠깐 정지하기
        Debug.Log($"{seconds}초 동안 잠자기 기능중");
        yield return new WaitForSeconds(seconds);
        Debug.Log("끝");

        //idleMove 에서 온 명령일 때 시퀀스 진행하기
        if (!isIdleMove)
        {
            if (progressSequence)
            {
                ExecuteLocalizedAction();
            }
            else
            {
                PlayerControl.instance.LockPlayerControl = false;
                progressSequence = true;
            }
        }

        //슬립 풀기
        if (isIdleMove)
        {
            idleCurrentMode = processMode.idle;
            progressIdleSequence = true;
        }
        else
        {
            currentMode = processMode.idle;
        }
    }
}
