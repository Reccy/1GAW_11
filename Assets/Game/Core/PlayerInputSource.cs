using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInputSource : MonoBehaviour
{
    #region INPUTS
    private Vector3 m_inputMovement = Vector3.zero;
    private Vector3 m_inputAim = Vector3.zero;

    private bool m_brake = false;
    private bool m_action = false;
    private bool m_shoot = false;
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
        CalculateInputAim();

        m_brake = m_player.GetButton("Brake");

        if (!m_action)
            m_action = m_player.GetButtonDown("Action");

        if (!m_shoot)
            m_shoot = m_player.GetButtonDown("Shoot");
    }

    private void FixedUpdate()
    {
        TrySteerBoat();
        TryAimBoat();
        TryShootBoat();
        TryBrakeBoat();
        TryDock();

        m_action = false;
        m_shoot = false;
    }

    private void CalculateInputMovement()
    {
        var inputMovementRaw = m_player.GetAxis2D("MoveHorizontal", "MoveVertical");

        var cameraRight = m_camera.transform.right;
        var cameraForward = Vector3.Cross(cameraRight, Vector3.up);

        m_inputMovement = cameraForward * inputMovementRaw.y + cameraRight * inputMovementRaw.x;
    }

    private void CalculateInputAim()
    {
        var inputAimRaw = m_player.GetAxis2D("AimHorizontal", "AimVertical");

        var cameraRight = m_camera.transform.right;
        var cameraForward = Vector3.Cross(cameraRight, Vector3.up);

        m_inputAim = cameraForward * inputAimRaw.y + cameraRight * inputAimRaw.x;
    }

    private void TrySteerBoat()
    {
        if (m_boatMover == null)
            return;

        m_boatMover.Steer(m_inputMovement);
    }

    private void TryAimBoat()
    {
        if (m_boatMover == null)
            return;

        m_boatMover.Aim(m_inputAim);
    }

    private void TryShootBoat()
    {
        if (m_boatMover == null)
            return;

        if (m_shoot)
            m_boatMover.Shoot();
    }

    private void TryBrakeBoat()
    {
        if (m_boatMover == null)
            return;

        if (m_brake)
            m_boatMover.Brake();
    }

    private void TryDock()
    {
        if (m_boatMover == null)
            return;

        if (m_action)
            m_boatMover.Dock();
    }
}
