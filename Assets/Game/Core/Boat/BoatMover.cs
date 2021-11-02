using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.DebugExtensions;

public class BoatMover : MonoBehaviour
{
    private Vector3 m_steerInput = Vector3.zero;

    [SerializeField] private float m_accelerationForce = 5.0f;
    [SerializeField] private float m_rotationForce = 5.0f;
    [SerializeField] private Rigidbody m_modelRigidbody;
    [SerializeField] private Rigidbody m_movementRigidbody;

    private Bounds ModelBounds => m_movementRigidbody.GetComponentInChildren<Collider>().bounds;
    private Ray ColRay => new Ray(ModelBounds.center, Vector3.down);

    private void Awake()
    {
    }

    private void FixedUpdate()
    {
        Debug2.DrawCross(ModelBounds.center);

        if (!CheckOnWater())
        {
            DebugDraw(Color.red);
            return;
        }

        DebugDraw(Color.green);

        Vector3 targetDirection = m_movementRigidbody.velocity.normalized;
        Vector3 currentDirection = m_modelRigidbody.transform.forward;

        Debug2.DrawArrow(m_modelRigidbody.position, m_modelRigidbody.position + targetDirection, Color.green);
        Debug2.DrawArrow(m_modelRigidbody.position, m_modelRigidbody.position + currentDirection, Color.red);
        Debug2.DrawArrow(m_modelRigidbody.position + currentDirection, m_modelRigidbody.position + targetDirection, Color.blue);

        var rotation = Vector3.Cross(currentDirection, targetDirection) * m_rotationForce;

        m_modelRigidbody.AddTorque(new Vector3(0, rotation.y, 0), ForceMode.Acceleration);
        m_movementRigidbody.AddForce(m_steerInput * m_accelerationForce);
    }

    public void Steer(Vector3 vect)
    {
        m_steerInput = vect;
    }

    private void DebugDraw(Color c)
    {
        Debug2.DrawArrow(ColRay, 1.0f, c);
    }

    private bool CheckOnWater()
    {
        Ray ray = ColRay;
        RaycastHit hitInfo;

        Physics.Raycast(ray, out hitInfo, 1.0f);

        if (hitInfo.collider == null)
            return false;

        return hitInfo.collider.CompareTag("Waterplane");
    }
}
