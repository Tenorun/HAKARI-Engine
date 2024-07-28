using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ������Ʈ�� Ŭ����
public class DetectedObject
{
    // ������Ʈ, ���� Z ��ǥ, bottomY
    public GameObject obj;
    public float originZ;

    public float bottomY;

    // ������Ʈ ��� �Լ�
    public void InitializeObject(GameObject Object)
    {
        obj = Object;
        originZ = Object.transform.position.z;
        bottomY = Object.transform.position.y - Object.transform.lossyScale.y / 2;
    }

    // Z ��ǥ ���� �Լ�
    public void AdjustZAxis(int adjustLevel)
    {
        float adjustedZ = originZ + (float)adjustLevel * 0.0001f;

        if (Mathf.Abs(obj.transform.position.z - adjustedZ) > 0.0001f)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, adjustedZ);
        }
    }

    //�ٴ� Y ���� �Լ�
    public void AdjustBottomY()
    {
        bottomY = obj.transform.position.y - obj.transform.lossyScale.y / 2;
    }

    // Z ��ġ �ʱ�ȭ �Լ�
    public void ResetZAxis()
    {
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, originZ);
    }
}

public class ZHandler : MonoBehaviour
{
    // ������ ������Ʈ���� ��ü ����Ʈ
    List<DetectedObject> mDetectedObjects = new List<DetectedObject>();
    // Rigidbody2D
    private Rigidbody2D rb;
    // �ν��Ͻ�
    public static ZHandler instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   // Rigidbody2D ������Ʈ �θ���
    }

    private void Update()
    {
        //�ٴ� Y �� ������Ʈ
        foreach (DetectedObject adjustObject in mDetectedObjects)
        {
            adjustObject.AdjustBottomY();
        }

        mDetectedObjects.Sort((a, b) => a.bottomY.CompareTo(b.bottomY));

        //Z ��ǥ ������Ʈ
        foreach (DetectedObject adjustObject in mDetectedObjects)
        {
            adjustObject.AdjustZAxis(FindDetectedObjectLocation(adjustObject.obj));
        }
    }

    // ��ü ����Ʈ���� Ÿ�� ������Ʈ�� ��ġ�� ��ġ ã��
    public int FindDetectedObjectLocation(GameObject targetObject)
    {
        // ã�� ��ġ (-1: �⺻��, ã�� ���ߴٴ� ��)
        int foundLocation = -1;

        // �ε�����
        int index = 0;
        foreach (DetectedObject searchPoint in mDetectedObjects)
        {
            // Ÿ�� ������Ʈ�� ã������ ã�� ��ġ�� �ε����� �ϰ� Ž�� ����, �ƴϸ� �ε��� 1 ���ϱ�
            if (searchPoint.obj == targetObject)
            {
                foundLocation = index;
                break;
            }
            else
            {
                index++;
            }
        }

        // ��ġ�� ��ȯ
        return foundLocation;
    }

    // ���� ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // "Unhand Z" �±װ� ������ ��ü ������ ����Ʈ �Ҵ��ϱ�
        if (!collision.gameObject.CompareTag("Unhand Z"))
        {
            // ��ü �����
            DetectedObject enterObject = new DetectedObject();
            enterObject.InitializeObject(collision.gameObject);

            // Z ��ǥ ����
            enterObject.AdjustZAxis(FindDetectedObjectLocation(collision.gameObject));

            // ����Ʈ�� ������Ʈ �߰�
            mDetectedObjects.Add(enterObject);
        }
    }

    // ���� ��
    private void OnTriggerExit2D(Collider2D collision)
    {
        // "Unhand Z" �±װ� ������ ������Ʈ �� ��ġ�� �ǵ����� ����Ʈ �Ҵ� �����ϱ�
        if (!collision.gameObject.CompareTag("Unhand Z"))
        {
            // ����Ʈ���� ��ü�� ��ġ ã��
            int location = FindDetectedObjectLocation(collision.gameObject);

            // ��ġ�� ã������ �ش� ������Ʈ�� Z��ǥ �ʱ�ȭ, ����Ʈ ��� ����
            if (location != -1)
            {
                mDetectedObjects[location].ResetZAxis();
                mDetectedObjects.RemoveAt(location);
            }
            else
            {
                Debug.LogWarning($"���� ������Ʈ: {collision.gameObject}(��)�� {gameObject.name}(��)�� �������, �̰��� ��ü ����Ʈ ������ ��� ��ġ�ϴ��� �� �� �������ϴ�.");
            }
        }
    }
}
