using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private float m_timeoutSeconds = 3.0f;
    [SerializeField] private float m_damageRadius = 3.0f;
    [SerializeField] private SphereCollider m_damageRadiusObj;

    private void Awake()
    {
        m_damageRadiusObj.transform.localScale *= m_damageRadius;
    }

    private void FixedUpdate()
    {
        m_timeoutSeconds -= Time.deltaTime;

        if (!m_damageRadiusObj.enabled)
        {
            if (m_timeoutSeconds < 0)
                JustFuckinExplode();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void JustFuckinExplode()
    {
        m_damageRadiusObj.enabled = true;
    }
}
