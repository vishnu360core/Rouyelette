using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public enum CameraSwitch{ wheel,table};

    [Header("Vcams:")]
    [SerializeField] List<CinemachineVirtualCamera> _vCams = new List<CinemachineVirtualCamera>();  

    [Header("Camera Switch:")] 
    [SerializeField] CameraSwitch _cameraSwitch; 

    // Start is called before the first frame update
    void Start()
    {
        
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CameraSwitchAction(CameraSwitch.wheel);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CameraSwitchAction(CameraSwitch.table);
        }
    }
}
