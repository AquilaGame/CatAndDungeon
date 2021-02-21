using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCharacterSelector : MonoBehaviour
{
    public enum SelectorType { CanvasRect, WorldCircle, WorldTargets, WorldNeighbor, WorldLocation }
    public delegate void AfterSelectBhv(List<CharacterScript> characters, Vector3 pos);
    [HideInInspector]public SelectorType type = SelectorType.WorldCircle;
    [HideInInspector]public bool isListening = false;
    [HideInInspector] public bool isRectCatching = false;
    List<CharacterScript> SelectedList = new List<CharacterScript>();
    Camera cam = null;
    [SerializeField] Canvas OnPlayCanvas = null;
    [SerializeField] GameObject RectSelectObj = null;
    GameObject RectRangeDisGo = null;
    [SerializeField] CharacterSelectVisrualObj CircleTargetObj = null;
    [SerializeField] CharacterSelectVisrualObj CircleRangeObj = null;
    Vector2 RectOrigin;
    AfterSelectBhv SelectCompleteBhv = null;
    CharacterScript nowCharacter = null;
    SkillStartScript.Filter nowFilter;
    float nowDistance;
    float nowRange;
    float nowHalfRange { get { return nowRange / 2.0f; } }
    int nowMaxCount;
    Vector3 nowWorldCirclePos
    {
        get { return CircleTargetObj.transform.position; }
    }
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    public Vector2 TranslateScreenPosToCanvas(Vector3 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(OnPlayCanvas.GetComponent<RectTransform>(), pos, cam, out Vector2 ret);
        return ret;
    }
    //把两个点转换成左下坐标和右上坐标
    void GetRectPoints(Vector2 v1, Vector2 v2, out Vector2 ov1, out Vector2 ov2)
    {
        ov1.x = v1.x > v2.x ? v2.x : v1.x;
        ov2.x = v1.x > v2.x ? v1.x : v2.x;
        ov1.y = v1.y > v2.y ? v2.y : v1.y;
        ov2.y = v1.y > v2.y ? v1.y : v2.y;
    }
    //框选
    public void StartRectCatch(SelectorType _type,AfterSelectBhv bhv = null)
    {
        type = _type;
        SelectedList.Clear();
        SelectCompleteBhv = bhv;
        isListening = true;
        OnGame.userCursor.CursorType = UserCursorType.AttackSearchTarget;
    }
    //选择目标
    public void StartCatch(CharacterScript cs, SkillStartScript.Filter filter, AfterSelectBhv bhv, float distance, int count)
    {
        //因为下面有了所以这里就不用再清一遍了
        //ResetRangePreview();
        type = SelectorType.WorldTargets;
        nowCharacter = cs;
        nowFilter = filter;
        nowDistance = distance;
        nowMaxCount = count;
        SelectedList.Clear();
        SelectCompleteBhv = bhv;
        isListening = true;
        OnGame.userCursor.CursorType = UserCursorType.AttackSearchTarget;
        if(!CircleRangeObj.gameObject.activeSelf)
            CircleRangeObj.gameObject.SetActive(true);
        CircleRangeObj.transform.parent = nowCharacter.transform;
        CircleRangeObj.Set(nowCharacter.transform.position, distance * 2);
    }
    //选择周围
    public void StartCatch(CharacterScript cs, SkillStartScript.Filter filter, AfterSelectBhv bhv, float range)
    {
        ResetRangePreview();
        type = SelectorType.WorldNeighbor;
        nowCharacter = cs;
        nowFilter = filter;
        nowRange = range;
        SelectedList.Clear();
        SelectCompleteBhv = bhv;
        isListening = true;
        OnGame.userCursor.CursorType = UserCursorType.AttackSearchTarget;
        CircleTargetObj.gameObject.SetActive(true);
        CircleTargetObj.Set(nowCharacter.transform.position, range * 2);
    }
    //选择区域
    public void StartCatch(CharacterScript cs, SkillStartScript.Filter filter, AfterSelectBhv bhv, float distance, float range)
    {
        //因为下面有了所以这里就不用再清一遍了
        //ResetRangePreview();
        type = SelectorType.WorldCircle;
        nowCharacter = cs;
        nowFilter = filter;
        nowDistance = distance;
        nowRange = range;
        SelectedList.Clear();
        SelectCompleteBhv = bhv;
        isListening = true;
        OnGame.userCursor.CursorType = UserCursorType.AttackSearchTarget;
        if (!CircleRangeObj.gameObject.activeSelf)
            CircleRangeObj.gameObject.SetActive(true);
        CircleRangeObj.transform.parent = nowCharacter.transform;
        CircleRangeObj.Set(nowCharacter.transform.position, distance * 2);
    }
    //选择位置
    public void StartLocationCatch(AfterSelectBhv bhv)
    {
        ResetRangePreview();
        type = SelectorType.WorldLocation;
        SelectedList.Clear();
        SelectCompleteBhv = bhv;
        isListening = true;
        OnGame.userCursor.CursorType = UserCursorType.AttackSearchTarget;
    }

    public static bool CheckDistance(Vector3 pos1, Vector3 pos2, float maxDis)
    {
        Vector3 res = new Vector3(pos1.x - pos2.x, 0, pos1.z - pos2.z);
        return res.magnitude <= maxDis;
    }

    public static bool CheckAlignment(SelectorType type, Character A, Character B, SkillStartScript.Filter filter)
    {
        //如果选择器是“自身周围单位”，我觉得就不该包括自己
        if (type == SelectorType.WorldNeighbor && A == B)
            return false;

        switch (filter)
        {
            case SkillStartScript.Filter.All:
                return true;
            case SkillStartScript.Filter.Friendly:
                return A.characterType == B.characterType;
            case SkillStartScript.Filter.Enemy:
                return A.characterType != B.characterType;
            default:
                return false;
        }
    }

    //选择逻辑：
    //WorldTargets：先改变指针类型，然后在要选择的人物上进行高亮，撤出人物时取消高亮，按键后触发回调，如果有下一个就继续选下一个没有就触发CallBack
    //WorldNeighbor：打开区域高亮，监听鼠标，按下左键时确定，扫描附近人物
    //WorldCircle：指针指向任意点时在那一点显示高亮，按下左键时确定，扫描人物
    //WorldLocation: 同上
    //CanvasRect：按下监听，移动监听，抬起监听，扫描人物

    public void StaticBlockListener(OnPlayMouseCtrl.ClickType clickType, StaticBlock staticBlock, Vector3 pointToPosition)
    {
        switch (clickType)
        {
            case OnPlayMouseCtrl.ClickType.Left:
                switch (type)
                {
                    case SelectorType.WorldNeighbor:
                        NeighborCallback();
                        break;
                    case SelectorType.WorldCircle:
                        WorldCircleCallback();
                        break;
                    case SelectorType.WorldLocation:
                        WorldLocationCallback();
                        break;
                    default:
                        break;
                }
                break;
            case OnPlayMouseCtrl.ClickType.Right:
                CancelSelect();
                break;
            case OnPlayMouseCtrl.ClickType.Nul:
                switch (type)
                {
                    case SelectorType.WorldCircle:
                        CircleTargetObj.gameObject.SetActive(true);
                        //如果允许精确定位AOE的释放位置就用这句
                        CircleTargetObj.Set(pointToPosition, nowRange, nowCharacter.transform.position, nowDistance);
                        //如果需要以每个格中心为基准算的AOE就要用下面这句
                        //CircleTargetObj.Set(staticBlock.transform.position, nowRange, nowCharacter.transform.position, nowDistance);
                        break;
                    case SelectorType.WorldTargets:
                        CircleTargetObj.gameObject.SetActive(false);
                        break;
                    case SelectorType.WorldLocation:
                        CircleTargetObj.gameObject.SetActive(true);
                        CircleTargetObj.Set(pointToPosition,1.0f);
                        break;
                    default: break;
                }
                break;
        }
    }
    public void MapObjectListener(OnPlayMouseCtrl.ClickType clickType, StaticBlock staticBlock, Vector3 pointToPosition)
    {
        switch (clickType)
        {
            case OnPlayMouseCtrl.ClickType.Left:
                switch (type)
                {
                    case SelectorType.WorldNeighbor:
                        NeighborCallback();
                        break;
                    case SelectorType.WorldCircle:
                        WorldCircleCallback();
                        break;
                    case SelectorType.WorldLocation:
                        WorldLocationCallback();
                        break;
                    default:
                        break;
                }
                break;
            case OnPlayMouseCtrl.ClickType.Right:
                CancelSelect();
                break;
            case OnPlayMouseCtrl.ClickType.Nul:
                switch (type)
                {
                    case SelectorType.WorldCircle:
                        CircleTargetObj.gameObject.SetActive(true);
                        //如果允许精确定位AOE的释放位置就用这句
                        CircleTargetObj.Set(new Vector3(pointToPosition.x, staticBlock.transform.position.y, pointToPosition.z), nowRange, nowCharacter.transform.position, nowDistance);
                        //如果需要以每个格中心为基准算的AOE就要用下面这句
                        //CircleTargetObj.Set(staticBlock.transform.position, nowRange, nowCharacter.transform.position, nowDistance);
                        break;
                    case SelectorType.WorldTargets:
                        CircleTargetObj.gameObject.SetActive(false);
                        break;
                    case SelectorType.WorldLocation:
                        CircleTargetObj.gameObject.SetActive(true);
                        CircleTargetObj.Set(pointToPosition, 1.0f);
                        break;
                    default: break;
                }
                break;
        }
    }
    public void SummonObjListener(OnPlayMouseCtrl.ClickType clickType, SummonObjectBase summonObject, Vector3 pointToPosition)
    {
        switch (clickType)
        {
            case OnPlayMouseCtrl.ClickType.Left:
                switch (type)
                {
                    case SelectorType.WorldNeighbor:
                        NeighborCallback();
                        break;
                    case SelectorType.WorldCircle:
                        WorldCircleCallback();
                        break;
                    default:
                        break;
                }
                break;
            case OnPlayMouseCtrl.ClickType.Right:
                CancelSelect();
                break;
            case OnPlayMouseCtrl.ClickType.Nul:
                switch (type)
                {
                    case SelectorType.WorldCircle:
                        CircleTargetObj.gameObject.SetActive(true);
                        //如果允许精确定位AOE的释放位置就用这句
                        CircleTargetObj.Set(pointToPosition, nowRange, nowCharacter.transform.position, nowDistance);
                        //如果需要以每个格中心为基准算的AOE就要用下面这句
                        //CircleTargetObj.Set(staticBlock.transform.position, nowRange, nowCharacter.transform.position, nowDistance);
                        break;
                    case SelectorType.WorldTargets:
                        CircleTargetObj.gameObject.SetActive(false);
                        break;
                    case SelectorType.WorldLocation:
                        CircleTargetObj.gameObject.SetActive(false);
                        break;
                    default: break;
                }
                break;
        }
    }
    public void SetRangePreview(CharacterScript character, float range)
    {
        if (isListening)
        {
            return;
        }
        CircleRangeObj.gameObject.SetActive(true);
        CircleRangeObj.transform.parent = character.transform;
        CircleRangeObj.Set(character.transform.position, range);
    }
    public void ResetRangePreview()
    {
        if (isListening)
        {
            return;
        }
        CircleRangeObj.transform.parent = null;
        CircleRangeObj.gameObject.SetActive(false);
    }
    public void CharacterListener(OnPlayMouseCtrl.ClickType clickType, CharacterScript characterScript)
    {
        switch (clickType)
        {
            case OnPlayMouseCtrl.ClickType.Left:
                switch (type)
                {
                    case SelectorType.WorldNeighbor:
                        NeighborCallback();
                        break;
                    case SelectorType.WorldCircle:
                        WorldCircleCallback();
                        break;
                    case SelectorType.WorldTargets:
                        if (CheckAlignment(type,nowCharacter.data, characterScript.data, nowFilter) &&
                            CheckDistance(nowCharacter.transform.position, characterScript.transform.position, nowDistance))
                        {
                            TargetSelectCallback(characterScript);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case OnPlayMouseCtrl.ClickType.Right:
                CancelSelect();
                break;
            case OnPlayMouseCtrl.ClickType.Nul:
                switch (type)
                {
                    case SelectorType.WorldCircle:
                        CircleTargetObj.gameObject.SetActive(true);
                        CircleTargetObj.Set(characterScript.transform.position, nowRange, nowCharacter.transform.position, nowDistance);
                        break;
                    case SelectorType.WorldTargets:
                        if (CheckAlignment(type, nowCharacter.data, characterScript.data, nowFilter) &&
                            CheckDistance(nowCharacter.transform.position, characterScript.transform.position, nowDistance))
                        {
                            CircleTargetObj.gameObject.SetActive(true);
                            CircleTargetObj.Set(characterScript.transform.position, 3.0f);
                        }
                        break;
                }
                break;
        }
    }

    void NeighborCallback()
    {
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (CheckAlignment(type, nowCharacter.data, v.data, nowFilter) && 
                CheckDistance(nowCharacter.transform.position, v.transform.position, nowRange))
            {
                SelectedList.Add(v);
            }
        }
        List<CharacterScript> tempList = new List<CharacterScript>(SelectedList);
        Vector3 tempPos = nowCharacter.transform.position;
        CancelSelect();//不能把Cancel放在后面，因为必中+数值固定的技能是一串函数调用出来的，这样退栈时候会正好触发取消，非常糟糕（找这个bug找了半小时）
        SelectCompleteBhv?.Invoke(tempList, tempPos);
    }

    void WorldCircleCallback()
    {
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (CheckAlignment(type, nowCharacter.data, v.data, nowFilter) &&
                CheckDistance(nowWorldCirclePos, v.transform.position, nowHalfRange))
            {
                SelectedList.Add(v);
            }
        }
        List<CharacterScript> tempList = new List<CharacterScript>(SelectedList);
        Vector3 tempPos = nowWorldCirclePos;
        CancelSelect();//不能把Cancel放在后面，因为必中+数值固定的技能是一串函数调用出来的，这样退栈时候会正好触发取消，非常糟糕（找这个bug找了半小时）
        SelectCompleteBhv?.Invoke(tempList, tempPos);
    }

    void WorldLocationCallback()
    {
        Vector3 tempPos = nowWorldCirclePos;
        CancelSelect();
        SelectCompleteBhv?.Invoke(null, tempPos);
    }

    void TargetSelectCallback(CharacterScript cs)
    {
        SelectedList.Add(cs);
        if (SelectedList.Count >= nowMaxCount)
        {
            List<CharacterScript> tempList = new List<CharacterScript>(SelectedList);
            Vector3 tempPos = cs.transform.position;
            CancelSelect();//不能把Cancel放在后面，因为必中+数值固定的技能是一串函数调用出来的，这样退栈时候会正好触发取消，非常糟糕（找这个bug找了半小时）
            SelectCompleteBhv?.Invoke(tempList, tempPos);
        }
    }

    public void RectCatchOnMouseDown(Vector3 pos)
    {
        Vector2 RectPos = TranslateScreenPosToCanvas(pos);
        RectOrigin = RectPos;
        RectRangeDisGo = Instantiate(RectSelectObj, OnPlayCanvas.transform);
        RectRangeDisGo.GetComponent<RectTransform>().anchoredPosition = RectPos;
        isRectCatching = true;
    }
    public void RectCatchDuringMouseDown(Vector3 pos)
    {
        GetRectPoints(RectOrigin, TranslateScreenPosToCanvas(pos), out Vector2 vectorLD, out Vector2 vectorRU);
        RectRangeDisGo.GetComponent<RectTransform>().anchoredPosition = vectorLD;
        RectRangeDisGo.GetComponent<RectTransform>().sizeDelta = vectorRU - vectorLD;
    }
    public void RectCatchOnMouseUp(Vector2 pos)
    {
        GetRectPoints(RectOrigin, TranslateScreenPosToCanvas(pos), out Vector2 vectorLD, out Vector2 vectorRU);
        Destroy(RectRangeDisGo);
        foreach (CharacterScript cs in MapSet.nowCreator.characters)
        {
            Vector2 cv = TranslateScreenPosToCanvas(cam.WorldToScreenPoint(cs.transform.position));
            if (cv.x > vectorLD.x && cv.y > vectorLD.y && cv.x < vectorRU.x && cv.y < vectorRU.y)
            {
                SelectedList.Add(cs);
            }
        }
        SelectCompleteBhv?.Invoke(SelectedList,Vector3.zero);
        isRectCatching = false;
        CancelSelect();
    }
    public void CancelSelect()
    {
        isListening = false;
        nowCharacter = null;
        CircleRangeObj.transform.parent = null;
        CircleTargetObj.gameObject.SetActive(false);
        CircleRangeObj.gameObject.SetActive(false);
        OnGame.userCursor.CursorType = UserCursorType.Normal;
    }
}
