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
        // Gizmo ���� ����
        Gizmos.color = gizmoColor;

        // ��ü�� ��ġ ǥ��
        Gizmos.DrawSphere(transform.position, gizmoSize);

        // �̸� ǥ��
        Handles.color = gizmoColor;
        Handles.Label(transform.position + Vector3.up * 0.5f, gameObject.name);
    }
}
