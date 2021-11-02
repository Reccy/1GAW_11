using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInputSource : MonoBehaviour
{
    #region INPUTS
    private Vector3 m_inputMovementRaw = Vector3.zero;
    private Vector3 m_inputMovement = Vector3.zero;
    #endregion

    #region REWIRED
    private int m_playerId = 0;
    private Player m_player;
    #endregion

    private Camera m_camera;
    private BoatMover m_boatMover;

    private void Start()
    {
        m_player = ReInput.players.GetPlayer(m_playerId);
        m_camera = Camera.main;

        m_boatMover = GetComponent<BoatMover>();
    }

    private void Update()
    {
        CalculateInputMovement();
    }

    private void FixedUpdate()
    {
        TrySteerBoat();
    }

    private void CalculateInputMovement()
    {
        m_inputMovementRaw = m_player.GetAxis2D("MoveHorizontal", "MoveVertical");

        var cameraRight = m_camera.transform.right;
        var cameraForward = Vector3.Cross(cameraRight, Vector3.up);

        m_inputMovement = cameraForward * m_inputMovementRaw.y + cameraRight * m_inputMovementRaw.x;
    }

    private void TrySteerBoat()
    {
        if (m_boatMover == null)
            return;

        m_boatMover.Steer(m_inputMovement);
    }
}
