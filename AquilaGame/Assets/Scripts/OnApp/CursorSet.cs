using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UserCursorType
{
    Normal,
    AttackSearchTarget,
    AttackLockTarget,
    AttackCannotLock
}
public class UserCursor
{
    UserCursorType _cursorType;
    public UserCursorType CursorType
    {
        get { return _cursorType; }
        set
        {
            Cursor.SetCursor(OnGame.cursorLib.CursorList[(int)value], Vector2.zero, CursorMode.Auto);
            _cursorType = value;
        }
    }
}
