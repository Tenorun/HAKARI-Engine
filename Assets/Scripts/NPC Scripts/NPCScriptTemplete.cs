using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScriptTemplete : MonoBehaviour
{
    private Collider2D npcCollider;

    //������ ��ȣ(�⺻��: 0)
    int sequenceNum = 0;
    //������ ���� ����(�⺻��: true)
    bool progressSequence = true;

    public enum processMode
    {
        idle,
        say,
        move
    }

    public processMode currentMode; //���� ���

    private bool doIdleAct = true;  //idleAct ���� ����

    public Transform[] targetPosition; // �̵��� ��ǥ ��ġ�� ��Ÿ���� Transform

    public float moveSpeed = 2.0f; // �̵� �ӵ�

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
        }
    }


    //�⺻������ �ൿ(ex: ��θ� ���� ������)
    [SerializeField] int idleSequenceNum = 0;            //��� ���� ������ ��ȣ
    [SerializeField] bool progressIdleSequence = true;   //������ �Ѿ ��ȣ

    processMode idleCurrentMode = processMode.idle;
    void idleAct()
    {
        if (progressIdleSequence)
        {
            //������ ����
            switch (idleSequenceNum)
            {
                case 0:
                    idleSequenceNum++;
                    progressIdleSequence = false;
                    idleCurrentMode = processMode.move;
                    MoveToTarget(targetPosition[0]);
                    Debug.Log("����!");
                    break;
                case 1:
                    idleSequenceNum = 0;
                    progressIdleSequence = false;
                    idleCurrentMode = processMode.move;
                    MoveToTarget(targetPosition[1]);
                    Debug.Log("���ð���.");
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
            }
        }
    }

    //�� �ൿ ����
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

    //���� �ൿ
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
                Debug.LogWarning($"������ �� {sequenceNum}�� �ش��ϴ� �ൿ�� �����ϴ�!");
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
                Say("#pfp(0,5)#name(����)�ȳ��ϼ���.#endl()" +
                    "����, ������ AU���� ���ΰ��� �ð��ִ� ��� ����, �����Դϴ�.#endl()" +
                    "#pfp(0,2)����, ���� ����, �ൿ���� ���� ū ���ظ� ��ġ��, �Ǹ��� �帰 ��ϴ�, ģ���е鲲, �˼��մϴ�.#endl()" +
                    "���ݺ��ʹ�");
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
                Debug.LogWarning($"������ �� {sequenceNum}�� �ش��ϴ� �ൿ�� �����ϴ�!");
                break;
        }
    }


    //������ ����
    void CutSequence()
    {
        doIdleAct = true;
        progressSequence = false;
    }

    //���ϱ�
    void Say(string dialogueLine)
    {
        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }

    //Ÿ������ �ڷ���Ʈ
    void Teleport(Transform target)
    {
        transform.position = target.position;
        Debug.Log($"NPC has been teleported to {target.position}");
    }

    //Ÿ������ ������
    void MoveToTarget(Transform target)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        isMoving = true;
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine(target));
    }

    //Ÿ������ �����̴� ����
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
