using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    static CameraController instance;
    public static CameraController Instance { get { return instance; } }


    [SerializeField] CinemachineBrain brain;

    public enum CameraSwitch{ wheel,table};

    [Header("Vcams:")]
    [SerializeField] List<CinemachineVirtualCamera> _vCams = new List<CinemachineVirtualCamera>();  

    [Header("Camera Switch:")] 
    [SerializeField] CameraSwitch _cameraSwitch;

    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    /// <summary>
    /// Check whether it is in transition 
    /// </summary>
    /// <returns></returns>
    public bool Reached()
    {
        return !brain.IsBlending;
    }

    public void CameraSwitchAction(CameraSwitch cameraSwitch)
    {
        switch (cameraSwitch)
        {
            case CameraSwitch.wheel:
                _cameraSwitch = CameraSwitch.wheel;

                _vCams[0].Priority = 20;
                _vCams[1].Priority = 10;
                break;

            case CameraSwitch.table:
                _cameraSwitch = CameraSwitch.table;

                _vCams[0].Priority = 10;
                _vCams[1].Priority = 20;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
