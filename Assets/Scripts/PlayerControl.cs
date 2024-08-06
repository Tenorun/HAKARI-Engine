using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //인스턴스
    public static PlayerControl instance;

    public float moveSpeed; // 플레이어 움직임 속도
    const float DEFAULT_MOVE_SPEED = 3f;    //걷기 속도
    const float SPRINT_MOVE_SPEED = 6f;    //달리기 속도

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Variable to store movement direction

    public Animator anim;
    public float stareAngle = -90f;    //플레이어가 바라보는 각도(일반적으로 컨트롤의 방향의 각도다.)
    public bool isMoving;       //플레이어 움직임 여부

    public bool LockPlayerControl = false;  //플레이어 컨트롤 잠금 여부

    private float rayLength = 0.6f; // Length of the raycast

    public GameObject rayHitObject; //레이캐스트에 걸린 오브젝트
    public bool isSubmitPress = false;    //확인버튼 눌림 여부
    public bool isSubmitDown = false;   //확인버튼 누름 시작 여부
    public bool isSubmitUp = false;     //확인버튼 뗌 여부

    //플레이어 입력에 따라 위치 업데이트
    void UpdatePosition_PlayerControl()
    {
        //스프린트 입력이 되어 있으면 moveSpeed를 달리기 속도로
        if (Input.GetButton("Sprint")) moveSpeed = SPRINT_MOVE_SPEED;
        else moveSpeed = DEFAULT_MOVE_SPEED;

        //플레이어 움직이기
        float prevX = rb.position.x;
        float prevY = rb.position.y;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        // 플레이어가 향하는 방향에 따라 stareAngle 값 바꾸기
        if (movement != Vector2.zero)
        {
            stareAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        }
    }

    //플레이어 입력에 따라 애니메이션 업데이트
    void UpdateAnimation_PlayerControl()
    {
        anim.SetBool("is Moving", isMoving);
        anim.SetFloat("Stare Angle", stareAngle + 0.000001f);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   //Rigidbody2D 컴포넌트 부르기
        anim = GetComponent<Animator>();    //Animator 컴포넌트 부르기

        stareAngle = -90f;
        isMoving = false;
        LockPlayerControl = false;
        isSubmitPress = false;
        isSubmitDown = false;
    }

    void Update()
    {
        if (!LockPlayerControl)
        {
            // 플레이어 입력 받기
            movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input
            movement.y = Input.GetAxisRaw("Vertical"); // Get vertical input

            isSubmitPress = Input.GetButton("Submit");
            isSubmitDown = Input.GetButtonDown("Submit");
            isSubmitUp = Input.GetButtonUp("Submit");

            // 입력여부에 따라 isMoving을 참 거짓으로 설정
            if (movement.x == 0f && movement.y == 0f) isMoving = false;
            else isMoving = true;

            // Normalize movement vector to ensure consistent speed in all directions
            movement.Normalize();
        }
    }

    void LateUpdate()
    {
        if (!LockPlayerControl)
        {
            UpdatePosition_PlayerControl();
            UpdateAnimation_PlayerControl();
            PerformRaycast();
        }
    }

    void PerformRaycast()
    {
        float raycastAngle = 0;

        if(stareAngle <= 135 && stareAngle > 45) raycastAngle = 90;
        else if (stareAngle <= 45 && stareAngle > -45) raycastAngle = 0;
        else if (stareAngle <= -45 && stareAngle > -135) raycastAngle = -90;
        else if (stareAngle <= -135 || stareAngle > 135 ) raycastAngle = 180;

        // Calculate the direction from the angle
        Vector2 direction = new Vector2(Mathf.Cos(raycastAngle * Mathf.Deg2Rad), Mathf.Sin(raycastAngle * Mathf.Deg2Rad));

        // Perform the raycast
        RaycastHit2D[] hitArr = Physics2D.RaycastAll(new Vector2(rb.position.x, rb.position.y), direction, rayLength);
        
        //레이가 무엇에 맞았는지에 따라 표시하기
        if (hitArr.Length > 0)
        {
            for(int i = 0; i < hitArr.Length; i++)
            {
                if (hitArr[i].collider.gameObject != this.gameObject && hitArr[i].collider != null && hitArr[i].collider.gameObject != ZHandler.instance.gameObject)
                {
                    rayHitObject = hitArr[i].collider.gameObject;
                    break;
                }
                else
                {
                    rayHitObject = null;
                }
            }
        }
        else
        {
            rayHitObject = null;
        }
    }

    void OnDrawGizmos()
    {
        float raycastAngle = 0;

        if (stareAngle <= 135 && stareAngle > 45) raycastAngle = 90;
        else if (stareAngle <= 45 && stareAngle > -45) raycastAngle = 0;
        else if (stareAngle <= -45 && stareAngle > -135) raycastAngle = -90;
        else if (stareAngle <= -135 || stareAngle > 135) raycastAngle = 180;

        // Calculate the direction from the angle
        Vector2 direction = new Vector2(Mathf.Cos(raycastAngle * Mathf.Deg2Rad), Mathf.Sin(raycastAngle * Mathf.Deg2Rad));

        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw the ray
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y), direction * rayLength);
    }
}
