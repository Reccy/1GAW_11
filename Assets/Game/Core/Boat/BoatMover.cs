using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.DebugExtensions;

public class BoatMover : MonoBehaviour
{
    private Vector3 m_steerInput = Vector3.zero;
    private Vector3 m_aimInput = Vector3.zero;

    private bool m_brakeInput = false;

    private bool m_isPlayer = false;
    private bool m_isAiming = false;
    private bool m_isShooting = false;

    [Header("Movement")]
    [SerializeField] private float m_accelerationForce = 5.0f;
    [SerializeField] private float m_rotationForce = 5.0f;
    [SerializeField] private float m_maxTurnRadiusDegrees = 45;
    [SerializeField] private float m_maxStopRadiusDegrees = 30;
    [SerializeField] private Rigidbody m_modelRigidbody;
    [SerializeField] private Rigidbody m_movementRigidbody;

    [Header("UI")]
    [SerializeField] private GameObject m_dockUI;

    [Header("Mines")]
    [SerializeField] private GameObject m_minePrefab;
    [SerializeField] private float m_mineCooldown = 2.0f;

    [Header("VFX")]
    [SerializeField] private GameObject m_explosionVFXPrefab;

    private Bounds ModelBounds => m_movementRigidbody.GetComponentInChildren<Collider>().bounds;
    private Ray ColRay => new Ray(ModelBounds.center, Vector3.down);
    private Vector3 ForwardDir => Vector3.ProjectOnPlane(m_modelRigidbody.transform.forward, Vector3.up).normalized;
    private Vector3 SteerVector => Vector3.ProjectOnPlane(m_steerInput, Vector3.up);
    private Vector3 LeftConstraintDir => RotatedVector(ForwardDir, -m_maxTurnRadiusDegrees);
    private Vector3 RightConstraintDir => RotatedVector(ForwardDir, m_maxTurnRadiusDegrees);

    private Port m_currentPort = null;
    private bool InDockArea => m_currentPort != null;
    private bool m_isDocked = false;

    private float m_mineCooldownRemaining = 0;

    private GameManager m_manager;

    private void Awake()
    {
        m_isPlayer = GetComponent<PlayerInputSource>() != null;
        m_manager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Port p = other.GetComponentInParent<Port>();
        if (p != null)
        {
            m_currentPort = p;

            if (m_isPlayer)
                m_dockUI.SetActive(true);
            else
                m_manager.EndGame();
        }
        else if (other.GetComponentInParent<Mine>())
        {
            BlowUp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Port p = other.GetComponentInParent<Port>();
        if (p != null)
        {
            m_currentPort = null;
            
            if (m_isPlayer)
                m_dockUI.SetActive(false);
        }

        if (m_isDocked)
            Undock();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<BoatMover>())
            BlowUp();
    }

    private void FixedUpdate()
    {
        Debug2.DrawCross(ModelBounds.center);

        if (!CheckOnWater())
        {
            BlowUp();
            return;
        }

        DebugDraw(Color.green);

        Vector3 targetDirection = m_movementRigidbody.velocity.normalized;
        Vector3 currentDirection = m_modelRigidbody.transform.forward;

        var rotation = Vector3.Cross(currentDirection, targetDirection) * m_rotationForce;

        // Yaw
        m_modelRigidbody.AddTorque(rotation, ForceMode.Acceleration);

        // Movement

        if (m_brakeInput || m_isDocked)
        {
            m_movementRigidbody.velocity *= 0.95f;
        }
        else
        {
            Vector3 movementDir = CalculateMovementDir();
            m_movementRigidbody.AddForce(movementDir * m_accelerationForce);
        }

        m_brakeInput = false;

        // Aiming

        if (m_isShooting)
        {
            PlaceMine();
        }

        m_isShooting = false;

        m_mineCooldownRemaining -= Time.deltaTime;
    }

    public void Steer(Vector3 vect)
    {
        m_steerInput = vect;
    }

    public void Aim(Vector3 vect)
    {
        if (vect.sqrMagnitude == 0)
        {
            m_isAiming = false;
            return;
        }

        m_isAiming = true;
        m_aimInput = vect.normalized;
    }

    public void Shoot()
    {
        m_isShooting = true;
    }

    public void Brake()
    {
        m_brakeInput = true;
    }

    public void Dock()
    {
        Debug.Log("dock invoked");

        if (!InDockArea)
            return;

        if (m_isDocked)
        {
            Undock();
            return;
        }

        Debug.Log($"Docket at {m_currentPort.GetPortName()}");
        m_isDocked = true;
    }

    private void Undock()
    {
        Debug.Log("undocked");
        m_isDocked = false;
    }

    private void PlaceMine()
    {
        if (m_mineCooldownRemaining > 0)
            return;

        m_mineCooldownRemaining = m_mineCooldown;
        Instantiate(m_minePrefab, ModelBounds.center, Quaternion.identity, null);
    }

    private void BlowUp()
    {
        Instantiate(m_explosionVFXPrefab, ModelBounds.center, Quaternion.identity, null);
        Destroy(gameObject);

        if (!m_isPlayer)
            m_manager.IncrementScore(10);
        else
            m_manager.EndGame();
    }

    private Vector3 CalculateMovementDir()
    {
        PolarCoordinate forwardPolar = V2P(ForwardDir, Vector3.right);
        PolarCoordinate steerPolar = V2P(SteerVector, ForwardDir);
        PolarCoordinate leftConstraintPolar = V2P(LeftConstraintDir, ForwardDir);
        PolarCoordinate rightConstraintPolar = V2P(RightConstraintDir, ForwardDir);

        steerPolar.theta = Mathf.Clamp(steerPolar.theta, rightConstraintPolar.theta, leftConstraintPolar.theta) + forwardPolar.theta;

        Vector3 steerVector = P2V(steerPolar);

        Debug2.DrawArrow(ModelBounds.center, ModelBounds.center + SteerVector);
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

        Physics.Raycast(ray, out hitInfo, 1.0f, LayerMask.GetMask("Water", "Default"));

        if (hitInfo.collider == null)
            return false;

        return hitInfo.collider.CompareTag("Waterplane");
    }
}
