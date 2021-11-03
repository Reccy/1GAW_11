using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoatBrain : MonoBehaviour
{
    private Transform m_target;
    private BoatMover m_boat;

    private void Awake()
    {
        m_target = FindObjectOfType<Port>().transform;
        m_boat = GetComponentInChildren<BoatMover>();
    }

    private void FixedUpdate()
    {
        m_boat.Steer((m_target.position - transform.position).normalized);
    }
}
