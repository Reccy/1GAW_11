using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVFXCleanup : MonoBehaviour
{
    ParticleSystem m_ps;

    private void Awake()
    {
        m_ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (m_ps.isStopped)
            Destroy(gameObject);
    }
}
