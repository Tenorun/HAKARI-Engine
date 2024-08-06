using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScriptTemplete : MonoBehaviour
{
    //콜라이더(콜라 마시고 싶다)
    private Collider2D npcCollider;
    //애니메이터
    public Animator anim;

    public float stareAngle = -90f;
    public bool isWalking = false;

    //기본 걷기 애니메이션 핸들 여부
    public bool handleDefaultWalkAnim = true;


    public int ACT_PARAMETER = 0;

    //프로세스 모드
    public enum processMode
    {
        idle,
        say,
        ask,
        cutScene,
        sleep
    }

    public processMode currentMode = processMode.idle; //현재 모드
    public bool interectionLock = false;

    // Start is called before the first frame update
    void Start()
    {
        //변수 초기화
        npcCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        ACT_PARAMETER = 0;

        currentMode = processMode.idle;
        interectionLock = false;

        stareAngle = -90f;
        isWalking = false;
        handleDefaultWalkAnim = true;

        //애니메이션 초기화
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
            //플레이어가 상호작용 하기 전의 행동
            //하는거: 플레이어 상호작용 체크, 대기상태 행동 등
            case processMode.idle:
                //플레이어 상호작용 체크
                if (!interectionLock && PlayerControl.instance.rayHitObject == this.gameObject && PlayerControl.instance.isSubmitDown)
                {
                    //여기에 플레이어 상호작용 시 할 행동을 넣기.
                    Debug.Log("플레이어 상호작용 함");
                    //예시~
                    PlayerControl.instance.LockPlayerControl = true;
                    
                    switch (ACT_PARAMETER)
                    {
                        case 0:
                            Say("Lorem ipsum odor amet, consectetuer adipiscing elit. Augue quisque eros sodales rutrum vehicula adipiscing potenti dis habitant.#endl()Maximus fermentum maecenas faucibus quis feugiat tristique magnis?", true);
                            break;
                        case 1:
                            Say("뭉탱이맨", true);
                            break;
                        default:
                            Debug.LogWarning($"ACT_PARAMETER, {ACT_PARAMETER}에 할당된 행동이 없습니다.");
                            break;
                    }
                    break;
                    //~예시
                }

                //대기상태 행동
                break;
            //말하기 모드 중의 행동
            //하는거: 말하기 끝남 감지 등
            case processMode.say:
                //대화상자 끝남 감지
                if (DialogueBoxScript.instance.currentMode == DialogueBoxScript.processMode.disabled)
                {
                    //여기에 대화상자 끝났을때 할 행동 넣기.

                    //예시~
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
                    //~예시
                }
                break;
            case processMode.ask:
                break;
            case processMode.cutScene:
                break;
            case processMode.sleep:
                break;
        }

        //애니메이션 업데이트
        if (handleDefaultWalkAnim)
        {
            anim.SetBool("is Moving", isWalking);
            anim.SetFloat("Stare Angle", stareAngle + 0.000001f);
        }
    }


    //말하기
    void Say(string dialogueLine, bool stopAndStarePlayer)
    {
        //가만히 서서 플레이어 바라보기 여부
        if (stopAndStarePlayer)
        {
            isWalking = false;
            Vector2 directionToTarget = PlayerControl.instance.gameObject.transform.position - transform.position; // 타겟 방향 벡터 계산
            stareAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // 각도 계산
        }
        Debug.Log("몬가몬가 말하고 있음");
        DialogueBoxScript.instance.GetInputMessage(dialogueLine);
        currentMode = processMode.say;
    }
}
