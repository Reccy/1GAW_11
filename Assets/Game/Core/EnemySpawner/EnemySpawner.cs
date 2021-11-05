using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float m_ySpawnPosition;
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private BoxCollider m_collider;
    private Vector3 SpawnBoundsMax => m_collider.transform.TransformPoint(m_collider.center + m_collider.size * 0.5f);
    private Vector3 SpawnBoundsMin => m_collider.transform.TransformPoint(m_collider.center - m_collider.size * 0.5f);

    private float m_timeUntilNextSpawn = 5.0f;
    private float m_spawnPaddingMinSeconds = 5.0f;
    private float m_spawnPaddingMaxSeconds = 10.0f;

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
        float x = Random.Range(SpawnBoundsMin.x, SpawnBoundsMax.x);
        float z = Random.Range(SpawnBoundsMin.z, SpawnBoundsMax.z);

        Vector3 spawnPosition = new Vector3(x, m_ySpawnPosition, z);

        Instantiate(m_enemyPrefab, spawnPosition, Quaternion.identity, null);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Debug.Log($"Min: {SpawnBoundsMin}, Max: {SpawnBoundsMax}");

        Gizmos.DrawSphere(SpawnBoundsMin, 1.0f);
        Gizmos.DrawSphere(SpawnBoundsMax, 1.0f);
    }
}
