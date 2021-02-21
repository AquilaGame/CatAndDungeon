using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    public float CamMoveSpd = 5.0f;
    public float CamZoomSpd = 2.0f;
    public Camera cam = null;
    OnPlayMouseCtrl detector = null;
    private void Awake()
    {
        OnScene.mainCam = this;
        OnGame.Log("主摄像机就位");
    }
    private void Start()
    {
        detector = GetComponent<OnPlayMouseCtrl>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MapSet.SetWorkMap(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MapSet.SetWorkMap(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MapSet.SetWorkMap(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MapSet.SetWorkMap(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MapSet.SetWorkMap(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MapSet.SetWorkMap(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            MapSet.SetWorkMap(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            MapSet.SetWorkMap(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            MapSet.SetWorkMap(8);
        }
    }
    void LateUpdate()
    {
        if(OnScene.isPlayMode)
        {
            if (detector.isUI)
                return;
            Vector3 MoveDelta = Vector3.zero;
            float MouseXval = (Input.GetButton("DragCam") ? 1.0f : 0.0f) * Input.GetAxis("Mouse X");
            float MouseYval = (Input.GetButton("DragCam") ? 1.0f : 0.0f) * Input.GetAxis("Mouse Y");

            if (Input.GetButton("FrontMove"))
            {
                MoveDelta += Vector3.forward * CamMoveSpd * Time.deltaTime;
            }
            else if (Input.GetButton("BackMove"))
            {
                MoveDelta += Vector3.back * CamMoveSpd * Time.deltaTime;
            }
            MoveDelta += Vector3.back * MouseYval;

            if (Input.GetButton("LeftMove"))
            {
                MoveDelta += Vector3.left * CamMoveSpd * Time.deltaTime;
            }
            else if (Input.GetButton("RightMove"))
            {
                MoveDelta += Vector3.right * CamMoveSpd * Time.deltaTime;
            }
            MoveDelta += Vector3.left * MouseXval;

            transform.position += MoveDelta;
            float axis = Input.GetAxis("Mouse ScrollWheel");
            if (axis > 0)
                cam.fieldOfView -= CamZoomSpd;
            else if(axis < 0)
                cam.fieldOfView += CamZoomSpd;
        }
    }
}
