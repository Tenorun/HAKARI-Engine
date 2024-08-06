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


    // ������ ��ȣ(�⺻��: 0)
    int sequenceNum = 0;
    // ������ ���� ����(�⺻��: true)
    bool progressSequence = true;

    public enum processMode
    {
        idle,
        say,
        move,
        sleep
    }

    public processMode currentMode; // ���� ���

    private bool doIdleAct = true;  // idleAct ���� ����

    public Transform[] targetPosition; // �̵��� ��ǥ ��ġ�� ��Ÿ���� Transform

    private float moveSpeed = 3.0f; // �̵� �ӵ�

    [SerializeField] bool isMoving = false; // �̵� ������ ����

    private Coroutine moveCoroutine; // �̵� �ڷ�ƾ ���� ����


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



        //�⺻ �ȱ� �ִϸ��̼� ������Ʈ
        if (handleDefaultWalkAnim)
        {
            anim.SetBool("is Moving", isWalking);
            anim.SetFloat("Stare Angle", stareAngle + 0.000001f);
        }
    }


    // �⺻������ �ൿ(ex: ��θ� ���� ������)
    [SerializeField] int idleSequenceNum = 0;            // ��� ���� ������ ��ȣ
    [SerializeField] bool progressIdleSequence = true;   // ������ �Ѿ ��ȣ

    processMode idleCurrentMode = processMode.idle;
    void idleAct()
    {
        if (progressIdleSequence)
        {
            progressIdleSequence = false;
            // ������ ����
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
                    Debug.LogWarning($"{idleSequenceNum}�� �ش��ϴ� ��� ���� �ൿ�� ���� 0���� �����մϴ�.");
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

    // �� �ൿ ����
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
                Debug.LogError($"�� {GameManagerScript.instance.languageVal}�� ���� �ൿ�� �����ϴ�!");
                break;
        }
    }

    // ���� �ൿ
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
                Debug.LogWarning($"������ �� {sequenceNum}�� �ش��ϴ� �ൿ�� �����ϴ�!");
                sequenceNum = 0;
                Debug.LogWarning($"������ ���� 0���� ���ư��ϴ�.");
                break;
        }
    }

    //�ѱ��� �ൿ
    void KoreanAct()
    {
        doIdleAct = false;
        PlayerControl.instance.LockPlayerControl = true;
        PlayerControl.instance.anim.SetBool("is Moving", false);

        switch (sequenceNum)
        {
            case 0:
                Say("#pfp(1,1)#name(���)�ȳ��ϼ���.#endl()" +
                    "����, ���𸮿��� ���ΰ��� �ð��ִ� ���� ����, ��� �Դϴ�.#endl()" +
                    "#pfp(1,2)����, ���� ����, �ൿ���� ���� ū ���ظ� ��ġ��, �Ǹ��� �帰 ������, ģ���е鲲, �˼��մϴ�.", true);
                sequenceNum++;
                break;
            case 1:
                Say("#pfp(1,1)#name(���)���ݺ��ʹ�", true);
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
                Debug.LogWarning($"������ �� {sequenceNum}�� �ش��ϴ� �ൿ�� �����ϴ�!");
                sequenceNum = 0;
                Debug.LogWarning($"������ ���� 0���� ���ư��ϴ�.");
                break;
        }
    }

    // ������ ����
    void CutSequence()
    {
        doIdleAct = true;
        progressSequence = false;
    }

    // ���ϱ�
    void Say(string dialogueLine, bool stopAndStare)
    {
        if (stopAndStare)
        {
            isWalking = false;
            Vector2 directionToTarget = PlayerControl.instance.gameObject.transform.position - transform.position; // Ÿ�� ���� ���� ���
            stareAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // ���� ���
        }

        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }

    // Ÿ������ �ڷ���Ʈ
    void Teleport(Transform target)
    {
        transform.position = target.position;
    }

    // Ÿ������ ������
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

        // �� ������Ʈ���� target�� ���ϴ� ������ ���ؼ� stareAngle�� ����
        Vector2 directionToTarget = target.position - transform.position; // Ÿ�� ���� ���� ���
        stareAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // ���� ���

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


    // Ÿ������ �����̴� ����
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

    // ���ڱ�
    IEnumerator Sleep(float seconds, bool isIdleMove)
    {
        //���� �ɱ�
        if (isIdleMove) idleCurrentMode = processMode.sleep;
        else currentMode = processMode.sleep;

        //��� �����ϱ�
        Debug.Log($"{seconds}�� ���� ���ڱ� �����");
        yield return new WaitForSeconds(seconds);
        Debug.Log("��");

        //idleMove ���� �� ����� �� ������ �����ϱ�
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

        //���� Ǯ��
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
