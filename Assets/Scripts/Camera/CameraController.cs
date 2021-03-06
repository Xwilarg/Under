using Cinemachine;
using UnityEngine;
using VarVarGamejam.Effect;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; } // static singleton

    [SerializeField]
    private CinemachineVirtualCamera cam2DTop;
    [SerializeField]
    private CinemachineVirtualCamera cam2DPlayer;
    [SerializeField]
    private CinemachineVirtualCamera cam3DPlayer;
    [SerializeField]
    private Camera cameraSettings;
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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraMode = CameraMode.m_2DTop;
        SwitchPriority((int)cameraMode);
    }

    public void SetPlayerCamera(Transform player, Transform head)
    {
        cam2DPlayer.Follow = player;
        cam2DTop.Follow = player;
        cam3DPlayer.Follow = head;

        cam2DPlayer.LookAt = player;
        cam2DTop.LookAt = player;
        cam3DPlayer.LookAt = head;
    }

    public Shake getCameraShake()
    {
        return cameraSettings.GetComponent<Shake>();
    }

    public void SwitchPriority(int value)
    {
        cameraMode = (CameraMode)value;
        if (cameraMode == CameraMode.m_2DTop)
        {
            cam2DTop.Priority = 2;
            cam2DPlayer.Priority = 1;
            cam3DPlayer.Priority = 0;
            cameraSettings.orthographic = true;
            sourceLight.shadows = LightShadows.None;
        }
        else if (cameraMode == CameraMode.m_2DPlayer)
        {
            cam2DTop.Priority = 1;
            cam2DPlayer.Priority = 2;
            cam3DPlayer.Priority = 0;
            cameraSettings.orthographic = true;
            sourceLight.shadows = LightShadows.None;
        }
        else if (cameraMode == CameraMode.m_3DPlayer)
        {
            cam2DTop.Priority = 1;
            cam2DPlayer.Priority = 0;
            cam3DPlayer.Priority = 2;
            cameraSettings.orthographic = false;
            cameraSettings.cullingMask = ~((1 << 6) | (1 << 7));
            cam2DPlayer.m_Lens.Orthographic = false;
            sourceLight.shadows = LightShadows.Hard;
        }
    }
}
