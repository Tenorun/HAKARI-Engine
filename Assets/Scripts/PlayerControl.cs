using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed; // 플레이어 움직임 속도
    const float DEFAULT_MOVE_SPEED = 6f;    //걷기 속도
    const float SPRINT_MOVE_SPEED = 13f;    //달리기 속도

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Variable to store movement direction


    private Animator anim;
    public float stareAngle = -90f;    //플레이어가 바라보는 각도(일반적으로 컨트롤의 방향의 각도다.)
    public bool isMoving;       //플레이어 움직임 여부

    public bool LockPlayerControl = false;  //플레이어 컨트롤 잠금 여부

    //플레이어 입력에 따라 위치 업데이트
    void UpdatePosition_PlayerControl()
    {
        //스프린트 입력이 되어 있으면 moveSpeed를 달리기 속도로
        if (Input.GetButton("Sprint")) moveSpeed = SPRINT_MOVE_SPEED;
        else moveSpeed = DEFAULT_MOVE_SPEED;

        //플레이어 움직이기
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
        anim.SetFloat("Stare Angle", stareAngle);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   //Rigidbody2D 컴포넌트 부르기
        anim = GetComponent<Animator>();    //Animator 컴포넌트 부르기

        stareAngle = -90f;
        isMoving = false;
        LockPlayerControl = false;
    }

    void Update()
    {
        if (!LockPlayerControl)
        {
            // 플레이어 입력 받기
            movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input
            movement.y = Input.GetAxisRaw("Vertical"); // Get vertical input

            // 입력여부에 따라 isMoving을 참 거짓으로 설정
            if (movement.x == 0f && movement.y == 0f) isMoving = false;
            else isMoving = true;

            // Normalize movement vector to ensure consistent speed in all directions
            movement.Normalize();
        }
    }

    void FixedUpdate()
    {
        if (!LockPlayerControl)
        {
            UpdatePosition_PlayerControl();
            UpdateAnimation_PlayerControl();
        }
    }
}
