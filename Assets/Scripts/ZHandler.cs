using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 감지된 오브젝트의 클래스
public class DetectedObject
{
    // 오브젝트, 원본 Z 좌표, bottomY
    public GameObject obj;
    public float originZ;

    public float bottomY;

    // 오브젝트 등록 함수
    public void InitializeObject(GameObject Object)
    {
        obj = Object;
        originZ = Object.transform.position.z;
        bottomY = Object.transform.position.y - Object.transform.lossyScale.y / 2;
    }

    // Z 좌표 조정 함수
    public void AdjustZAxis(int adjustLevel)
    {
        float adjustedZ = originZ + (float)adjustLevel * 0.0001f;

        if (Mathf.Abs(obj.transform.position.z - adjustedZ) > 0.0001f)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, adjustedZ);
        }
    }

    //바닥 Y 조정 함수
    public void AdjustBottomY()
    {
        bottomY = obj.transform.position.y - obj.transform.lossyScale.y / 2;
    }

    // Z 위치 초기화 함수
    public void ResetZAxis()
    {
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, originZ);
    }
}

public class ZHandler : MonoBehaviour
{
    // 감지된 오브젝트들의 객체 리스트
    List<DetectedObject> mDetectedObjects = new List<DetectedObject>();
    // Rigidbody2D
    private Rigidbody2D rb;
    // 인스턴스
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
        rb = GetComponent<Rigidbody2D>();   // Rigidbody2D 컴포넌트 부르기
    }

    private void Update()
    {
        //바닥 Y 값 업데이트
        foreach (DetectedObject adjustObject in mDetectedObjects)
        {
            adjustObject.AdjustBottomY();
        }

        mDetectedObjects.Sort((a, b) => a.bottomY.CompareTo(b.bottomY));

        //Z 좌표 업데이트
        foreach (DetectedObject adjustObject in mDetectedObjects)
        {
            adjustObject.AdjustZAxis(FindDetectedObjectLocation(adjustObject.obj));
        }
    }

    // 객체 리스트에서 타겟 오브젝트가 위치한 위치 찾기
    public int FindDetectedObjectLocation(GameObject targetObject)
    {
        // 찾은 위치 (-1: 기본값, 찾지 못했다는 뜻)
        int foundLocation = -1;

        // 인덱스값
        int index = 0;
        foreach (DetectedObject searchPoint in mDetectedObjects)
        {
            // 타겟 오브젝트를 찾았으면 찾은 위치를 인덱스로 하고 탐색 종료, 아니면 인덱스 1 더하기
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

        // 위치값 반환
        return foundLocation;
    }

    // 들어올 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // "Unhand Z" 태그가 없으면 객체 생성과 리스트 할당하기
        if (!collision.gameObject.CompareTag("Unhand Z"))
        {
            // 객체 만들기
            DetectedObject enterObject = new DetectedObject();
            enterObject.InitializeObject(collision.gameObject);

            // Z 좌표 조정
            enterObject.AdjustZAxis(FindDetectedObjectLocation(collision.gameObject));

            // 리스트에 오브젝트 추가
            mDetectedObjects.Add(enterObject);
        }
    }

    // 나갈 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        // "Unhand Z" 태그가 없으면 오브젝트 원 위치로 되돌리고 리스트 할당 해제하기
        if (!collision.gameObject.CompareTag("Unhand Z"))
        {
            // 리스트에서 객체의 위치 찾기
            int location = FindDetectedObjectLocation(collision.gameObject);

            // 위치를 찾았으면 해당 오브젝트의 Z좌표 초기화, 리스트 요소 제거
            if (location != -1)
            {
                mDetectedObjects[location].ResetZAxis();
                mDetectedObjects.RemoveAt(location);
            }
            else
            {
                Debug.LogWarning($"게임 오브젝트: {collision.gameObject}(이)가 {gameObject.name}(을)를 벗어났지만, 이것이 객체 리스트 변수의 어디에 위치하는지 알 수 없었습니다.");
            }
        }
    }
}
