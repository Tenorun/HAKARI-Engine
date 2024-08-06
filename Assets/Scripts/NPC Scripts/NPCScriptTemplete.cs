using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScriptTemplete : MonoBehaviour
{
    //�ݶ��̴�(�ݶ� ���ð� �ʹ�)
    private Collider2D npcCollider;
    //�ִϸ�����
    public Animator anim;

    public float stareAngle = -90f;
    public bool isWalking = false;

    //�⺻ �ȱ� �ִϸ��̼� �ڵ� ����
    public bool handleDefaultWalkAnim = true;


    public int ACT_PARAMETER = 0;

    //���μ��� ���
    public enum processMode
    {
        idle,
        say,
        ask,
        cutScene,
        sleep
    }

    public processMode currentMode = processMode.idle; //���� ���
    public bool interectionLock = false;

    // Start is called before the first frame update
    void Start()
    {
        //���� �ʱ�ȭ
        npcCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        ACT_PARAMETER = 0;

        currentMode = processMode.idle;
        interectionLock = false;

        stareAngle = -90f;
        isWalking = false;
        handleDefaultWalkAnim = true;

        //�ִϸ��̼� �ʱ�ȭ
        if (handleDefaultWalkAnim)
        {
            anim.SetBool("is Moving", isWalking);
            anim.SetFloat("Stare Angle", stareAngle + 0.000001f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            //�÷��̾ ��ȣ�ۿ� �ϱ� ���� �ൿ
            //�ϴ°�: �÷��̾� ��ȣ�ۿ� üũ, ������ �ൿ ��
            case processMode.idle:
                //�÷��̾� ��ȣ�ۿ� üũ
                if (!interectionLock && PlayerControl.instance.rayHitObject == this.gameObject && PlayerControl.instance.isSubmitDown)
                {
                    //���⿡ �÷��̾� ��ȣ�ۿ� �� �� �ൿ�� �ֱ�.
                    Debug.Log("�÷��̾� ��ȣ�ۿ� ��");
                    //����~
                    PlayerControl.instance.LockPlayerControl = true;
                    
                    switch (ACT_PARAMETER)
                    {
                        case 0:
                            Say("Lorem ipsum odor amet, consectetuer adipiscing elit. Augue quisque eros sodales rutrum vehicula adipiscing potenti dis habitant.#endl()Maximus fermentum maecenas faucibus quis feugiat tristique magnis?", true);
                            break;
                        case 1:
                            Say("�����̸�", true);
                            break;
                        default:
                            Debug.LogWarning($"ACT_PARAMETER, {ACT_PARAMETER}�� �Ҵ�� �ൿ�� �����ϴ�.");
                            break;
                    }
                    break;
                    //~����
                }

                //������ �ൿ
                break;
            //���ϱ� ��� ���� �ൿ
            //�ϴ°�: ���ϱ� ���� ���� ��
            case processMode.say:
                //��ȭ���� ���� ����
                if (DialogueBoxScript.instance.currentMode == DialogueBoxScript.processMode.disabled)
                {
                    //���⿡ ��ȭ���� �������� �� �ൿ �ֱ�.

                    //����~
                    PlayerControl.instance.LockPlayerControl = false;
                    currentMode = processMode.idle;

                    switch (ACT_PARAMETER)
                    {
                        case 0:
                            ACT_PARAMETER = 1;
                            break;
                        case 1:
                            ACT_PARAMETER = 0;
                            break;
                    }
                    break;
                    //~����
                }
                break;
            case processMode.ask:
                break;
            case processMode.cutScene:
                break;
            case processMode.sleep:
                break;
        }

        //�ִϸ��̼� ������Ʈ
        if (handleDefaultWalkAnim)
        {
            anim.SetBool("is Moving", isWalking);
            anim.SetFloat("Stare Angle", stareAngle + 0.000001f);
        }
    }


    //���ϱ�
    void Say(string dialogueLine, bool stopAndStarePlayer)
    {
        //������ ���� �÷��̾� �ٶ󺸱� ����
        if (stopAndStarePlayer)
        {
            isWalking = false;
            Vector2 directionToTarget = PlayerControl.instance.gameObject.transform.position - transform.position; // Ÿ�� ���� ���� ���
            stareAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // ���� ���
        }
        Debug.Log("�󰡸� ���ϰ� ����");
        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }
}
