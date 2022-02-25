using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField]
    private InputAction action;
    [SerializeField]
    private CinemachineVirtualCamera cam2DTop;
    [SerializeField]
    private CinemachineVirtualCamera cam2DPlayer;
    [SerializeField]
    private CinemachineVirtualCamera cam3DPlayer;
    [SerializeField]
    private Camera cameraSettings;
    [SerializeField]
    private GameObject player2D;
    [SerializeField]
    private Light sourceLight;

    [System.Serializable]
    public enum CameraMode
    {
        m_2DTop,
        m_2DPlayer,
        m_3DPlayer
    };
    [SerializeField]
    private CameraMode cameraMode;

    // Start is called before the first frame update
    void Start()
    {
        cameraMode = CameraMode.m_2DTop;
        SwitchPriority((int)cameraMode);
    }

    public void SwitchPriority(int value)
    {
        Debug.Log(value);
        cameraMode = (CameraMode)value;
        if (cameraMode == CameraMode.m_2DTop)
        {
            cam2DTop.Priority = 2;
            cam2DPlayer.Priority = 1;
            cam3DPlayer.Priority = 0;
            cameraSettings.orthographic = true;
            player2D.SetActive(true);
            sourceLight.shadows = LightShadows.None;

        }
        else if (cameraMode == CameraMode.m_2DPlayer)
        {
            cam2DTop.Priority = 1;
            cam2DPlayer.Priority = 2;
            cam3DPlayer.Priority = 0;
            cameraSettings.orthographic = true;
            player2D.SetActive(true);
            sourceLight.shadows = LightShadows.None;
        }
        else if (cameraMode == CameraMode.m_3DPlayer)
        {
            cam2DTop.Priority = 1;
            cam2DPlayer.Priority = 0;
            cam3DPlayer.Priority = 2;
            cameraSettings.orthographic = false;
            player2D.SetActive(false);
            sourceLight.shadows = LightShadows.Hard;
        }
    }
}
