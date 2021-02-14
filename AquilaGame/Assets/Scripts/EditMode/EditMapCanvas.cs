using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMapCanvas : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button CloseBtn;
    [SerializeField] GameObject PlayModeCanvas = null;
    void Start()
    {
        CloseBtn.onClick.AddListener(SetClose);
    }

    void SetClose()
    {
        OnScene.mainCam.cam.orthographic = false;
        MapSet.nowCreator.BuildMesh();
        PlayModeCanvas.SetActive(true);
        gameObject.SetActive(false);
        OnScene.isPlayMode = true;

    }
}
