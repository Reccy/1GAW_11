using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloatField : MonoBehaviour
{
    [SerializeField] private float m_bouyancyIntensity = 100.0f;

    private BoxCollider m_area;

    private void Awake()
    {
        m_area = GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
            return;

        float depth = CalculateDepth(rb);

        rb.AddForce(Vector3.up * m_bouyancyIntensity * depth, ForceMode.Acceleration);
    }

    private float CalculateDepth(Rigidbody rb)
    {
        return Mathf.Max(0, m_area.bounds.max.y - rb.centerOfMass.y);
    }
}
