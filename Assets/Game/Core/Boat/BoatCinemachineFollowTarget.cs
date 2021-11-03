using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCinemachineFollowTarget : MonoBehaviour
{
    [SerializeField] SphereCollider m_target;

    private void FixedUpdate()
    {
        transform.position = m_target.transform.TransformPoint(m_target.center);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, m_target.transform.rotation.z);
    }
}
