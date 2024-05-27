using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float moveSpeed; // �÷��̾� ������ �ӵ�
    const float DEFAULT_MOVE_SPEED = 6f;    //�ȱ� �ӵ�
    const float SPRINT_MOVE_SPEED = 13f;    //�޸��� �ӵ�

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Variable to store movement direction

    public float stareAngle;    //�÷��̾ �ٶ󺸴� ����(�Ϲ������� ��Ʈ���� ������ ������.)
    public bool isMoving;       //�÷��̾� ������ ����

    public bool LockPlayerControl = false;  //�÷��̾� ��Ʈ�� ��� ����

    //�÷��̾� �Է¿� ���� ��ġ ������Ʈ
    void UpdatePosition_PlayerControl()
    {
        //������Ʈ �Է��� �Ǿ� ������ moveSpeed�� �޸��� �ӵ���
        if (Input.GetButton("Sprint")) moveSpeed = SPRINT_MOVE_SPEED;
        else moveSpeed = DEFAULT_MOVE_SPEED;

        //�÷��̾� �����̱�
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // �÷��̾ ���ϴ� ���⿡ ���� stareAngle �� �ٲٱ�
        if (movement != Vector2.zero)
        {
            stareAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        }
    }

    //�÷��̾� �Է¿� ���� �ִϸ��̼� ������Ʈ
    void UpdateAnimation_PlayerControl()
    {

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        LockPlayerControl = false;
    }

    void Update()
    {
        if (!LockPlayerControl)
        {
            // �÷��̾� �Է� �ޱ�
            movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input
            movement.y = Input.GetAxisRaw("Vertical"); // Get vertical input

            // �Է¿��ο� ���� isMoving�� �� �������� ����
            if(movement.x == 0f && movement.y == 0f) isMoving = false;
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
