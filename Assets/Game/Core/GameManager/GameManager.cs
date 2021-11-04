using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_scoreUI;
    [SerializeField] private TMP_Text m_endGameUI;

    private float m_restartTimer = 10.0f;

    private int m_score = 0;
    private bool m_gameRunning = true;

    public void IncrementScore(int amount)
    {
        if (!m_gameRunning)
            return;

        m_score += amount;
        m_scoreUI.text = $"Score: {m_score}";
    }

    public void EndGame()
    {
        if (!m_gameRunning)
            return;

        m_gameRunning = false;
        m_endGameUI.enabled = true;
        m_endGameUI.text = $"GAME OVER!\n\nFinal Score: {m_score}";
        m_scoreUI.enabled = false;

        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        while (m_restartTimer > 0)
        {
            m_restartTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
