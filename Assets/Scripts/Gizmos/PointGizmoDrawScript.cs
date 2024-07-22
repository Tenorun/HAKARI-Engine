using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PointGizmoDrawScript : MonoBehaviour
{
    public Color gizmoColor = Color.cyan;
    public float gizmoSize = 0.2f;

    private void OnDrawGizmos()
    {
        // Gizmo 색상 설정
        Gizmos.color = gizmoColor;

        // 구체로 위치 표시
        Gizmos.DrawSphere(transform.position, gizmoSize);

        // 이름 표시
        Handles.color = gizmoColor;
        Handles.Label(transform.position + Vector3.up * 0.5f, gameObject.name);
    }
}
