using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float m_ySpawnPosition;
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private BoxCollider m_collider;
    private Bounds m_spawnBounds;

    private float m_timeUntilNextSpawn = 5.0f;
    private float m_spawnPaddingMinSeconds = 5.0f;
    private float m_spawnPaddingMaxSeconds = 10.0f;

    private void Awake()
    {
        m_spawnBounds = m_collider.bounds;
    }

    private void FixedUpdate()
    {
        m_timeUntilNextSpawn -= Time.deltaTime;

        if (m_timeUntilNextSpawn < 0)
        {
            m_timeUntilNextSpawn = Random.Range(m_spawnPaddingMinSeconds, m_spawnPaddingMaxSeconds);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        float x = Random.Range(m_spawnBounds.min.x, m_spawnBounds.max.x);
        float z = Random.Range(m_spawnBounds.min.z, m_spawnBounds.max.z);

        Vector3 spawnPosition = new Vector3(x, m_ySpawnPosition, z);

        Instantiate(m_enemyPrefab, spawnPosition, Quaternion.identity, null);
    }
}
