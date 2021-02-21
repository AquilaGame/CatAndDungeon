using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLib : MonoBehaviour
{
    public Texture2D[] CursorList;
    private void Awake()
    {
        OnGame.cursorLib = this;
        OnGame.Log("光标库连接成功");
    }
    private void Start()
    {
        OnGame.userCursor.CursorType = UserCursorType.Normal;
    }
}
