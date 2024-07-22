using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //�ν��Ͻ�
    public static PlayerControl instance;

    public float moveSpeed; // �÷��̾� ������ �ӵ�
    const float DEFAULT_MOVE_SPEED = 6f;    //�ȱ� �ӵ�
    const float SPRINT_MOVE_SPEED = 13f;    //�޸��� �ӵ�

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Variable to store movement direction

    public Animator anim;
    public float stareAngle = -90f;    //�÷��̾ �ٶ󺸴� ����(�Ϲ������� ��Ʈ���� ������ ������.)
    public bool isMoving;       //�÷��̾� ������ ����

    public bool LockPlayerControl = false;  //�÷��̾� ��Ʈ�� ��� ����

    private float rayLength = 1f; // Length of the raycast

    public GameObject rayHitObject; //����ĳ��Ʈ�� �ɸ� ������Ʈ
    public bool isSubmitPress = false;    //Ȯ�ι�ư ���� ����
    public bool isSubmitDown = false;   //Ȯ�ι�ư ���� ���� ����
    public bool isSubmitUp = false;     //Ȯ�ι�ư �� ����

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
        anim.SetBool("is Moving", isMoving);
        anim.SetFloat("Stare Angle", stareAngle);
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
        rb = GetComponent<Rigidbody2D>();   //Rigidbody2D ������Ʈ �θ���
        anim = GetComponent<Animator>();    //Animator ������Ʈ �θ���

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
            // �÷��̾� �Է� �ޱ�
            movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input
            movement.y = Input.GetAxisRaw("Vertical"); // Get vertical input

            isSubmitPress = Input.GetButton("Submit");
            isSubmitDown = Input.GetButtonDown("Submit");
            isSubmitUp = Input.GetButtonUp("Submit");

            // �Է¿��ο� ���� isMoving�� �� �������� ����
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
            PerformRaycast();
        }
    }

    void PerformRaycast()
    {
        // Calculate the direction from the angle
        Vector2 direction = new Vector2(Mathf.Cos(stareAngle * Mathf.Deg2Rad), Mathf.Sin(stareAngle * Mathf.Deg2Rad));

        // Perform the raycast
        RaycastHit2D[] hit = Physics2D.RaycastAll(rb.position, direction, rayLength);

        //���̰� ������ �¾Ҵ����� ���� ǥ���ϱ�
        if (hit.Length > 1)
        {
            if (hit[1].collider != null && hit[1].collider != this.gameObject)
            {
                rayHitObject = hit[1].collider.gameObject;
            }
            else
            {
                rayHitObject = null;
            }
        }
        else
        {
            rayHitObject = null;
        }
    }

    void OnDrawGizmos()
    {
        // Calculate the direction from the angle
        Vector2 direction = new Vector2(Mathf.Cos(stareAngle * Mathf.Deg2Rad), Mathf.Sin(stareAngle * Mathf.Deg2Rad));

        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw the ray
        Gizmos.DrawRay(transform.position, direction * rayLength);
    }
}
