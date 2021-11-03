using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private float m_timeoutSeconds = 3.0f;
    [SerializeField] private float m_damageRadius = 3.0f;
    [SerializeField] private GameObject m_damageRadiusObj;

    private void Awake()
    {
        m_damageRadiusObj.transform.localScale = Vector3.one * m_damageRadius;
    }

    private void FixedUpdate()
    {
        m_timeoutSeconds -= Time.deltaTime;

        if (m_timeoutSeconds < 0)
            JustFuckinExplode();
    }

    private void JustFuckinExplode()
    {
        Destroy(gameObject);
    }
}
