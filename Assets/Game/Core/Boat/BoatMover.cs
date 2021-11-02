using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.DebugExtensions;

public class BoatMover : MonoBehaviour
{
    private Vector3 m_steerInput = Vector3.zero;

    [SerializeField] private float m_accelerationForce = 5.0f;
    [SerializeField] private float m_rotationForce = 5.0f;
    [SerializeField] private float m_maxTurnRadiusDegrees = 45;
    [SerializeField] private Rigidbody m_modelRigidbody;
    [SerializeField] private Rigidbody m_movementRigidbody;

    private Bounds ModelBounds => m_movementRigidbody.GetComponentInChildren<Collider>().bounds;
    private Ray ColRay => new Ray(ModelBounds.center, Vector3.down);
    private Vector3 ForwardDir => Vector3.ProjectOnPlane(m_modelRigidbody.transform.forward, Vector3.up).normalized;
    private Vector3 SteerDir => Vector3.ProjectOnPlane(m_steerInput, Vector3.up).normalized;
    private Vector3 LeftConstraintDir => RotatedVector(ForwardDir, -m_maxTurnRadiusDegrees);
    private Vector3 RightConstraintDir => RotatedVector(ForwardDir, m_maxTurnRadiusDegrees);

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
        /*
        Debug2.DrawArrow(m_modelRigidbody.position, m_modelRigidbody.position + targetDirection, Color.green);
        Debug2.DrawArrow(m_modelRigidbody.position, m_modelRigidbody.position + currentDirection, Color.red);
        Debug2.DrawArrow(m_modelRigidbody.position + currentDirection, m_modelRigidbody.position + targetDirection, Color.blue);
        */
        var rotation = Vector3.Cross(currentDirection, targetDirection) * m_rotationForce;

        // Yaw
        m_modelRigidbody.AddTorque(rotation, ForceMode.Acceleration);

        // Movement
        Vector3 movementDir = CalculateMovementDir();
        m_movementRigidbody.AddForce(movementDir * m_accelerationForce);
    }

    public void Steer(Vector3 vect)
    {
        m_steerInput = vect;
    }

    private Vector3 CalculateMovementDir()
    {
        PolarCoordinate forwardPolar = V2P(ForwardDir, Vector3.right);
        PolarCoordinate steerPolar = V2P(SteerDir, ForwardDir);
        PolarCoordinate leftConstraintPolar = V2P(LeftConstraintDir, ForwardDir);
        PolarCoordinate rightConstraintPolar = V2P(RightConstraintDir, ForwardDir);

        steerPolar.theta = Mathf.Clamp(steerPolar.theta, rightConstraintPolar.theta, leftConstraintPolar.theta) + forwardPolar.theta;

        Vector3 steerVector = P2V(steerPolar);

        Debug2.DrawArrow(ModelBounds.center, ModelBounds.center + SteerDir);
        Debug2.DrawArrow(ModelBounds.center, ModelBounds.center + ForwardDir, Color.blue);
        Debug2.DrawArrow(ModelBounds.center, ModelBounds.center + LeftConstraintDir, Color.green);
        Debug2.DrawArrow(ModelBounds.center, ModelBounds.center + RightConstraintDir, Color.green);
        Debug2.DrawArrow(ModelBounds.center, ModelBounds.center + steerVector.normalized * 3, Color.red);

        return steerVector;
    }

    private struct PolarCoordinate
    {
        public float r;
        public float theta; // radians
    }

    private PolarCoordinate V2P(Vector3 from, Vector3 relative)
    {
        PolarCoordinate result = new PolarCoordinate();

        result.r = from.magnitude;
        result.theta = Vector3.SignedAngle(from, relative, Vector3.up) * Mathf.Deg2Rad;

        return result;
    }

    private Vector3 P2V(PolarCoordinate from)
    {
        Vector3 result = Vector3.zero;

        result.z = from.r * Mathf.Sin(from.theta);
        result.x = from.r * Mathf.Cos(from.theta);

        return result;
    }

    private Vector3 RotatedVector(Vector3 originDir, float degrees)
    {
        Vector3 axisDir = Vector3.up;

        Quaternion rot = Quaternion.AngleAxis(degrees, axisDir);
        return rot * originDir;
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
