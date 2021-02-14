using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;


public class OnPlayMouseCtrl : MonoBehaviour
{
    public enum ClickType
    {
        Nul = 0,
        Left = 1,
        Right = 2
    }
    [SerializeField] MousePanel mousePnl = null;
    [SerializeField] SelectVisualWithAnim moveTarget = null;
    [SerializeField] LineRenderer MovePreviewRoute = null;
    [SerializeField] ActionPointPanel AP_Panel = null;
    [SerializeField] GameObject TitleCanvas = null;
    Material targetPointMateral = null;
    public Color GreenRouteColor;
    public Color YellowRouteColor;
    public Color RedRouteColor;
    public Color GreenTargetColor;
    public Color YellowTargetColor;
    public Color RedTargetColor;
    public Vector3 LineOffset;
    bool isNeedMousePnlOn = false;
    bool isNeedRouteOn = false;
    bool isNeedTargetPointOn = false;
    bool isMousePnlOn = false;
    bool isRouteOn = false;
    bool isTargetPointOn = false;
    public bool isMousePnlVisable
    {
        get { return mousePnl.gameObject.activeSelf; }
        set
        {
            isMousePnlOn = value;
            mousePnl.gameObject.SetActive(value);
        }
    }
    public bool isRouteVisable
    {
        get { return MovePreviewRoute.gameObject.activeSelf; }
        set
        {
            isRouteOn = value;
            MovePreviewRoute.gameObject.SetActive(value);
        }
    }
    public bool isTargetPointVisable
    {
        get { return moveTarget.gameObject.activeSelf; }
        set
        {
            isTargetPointOn = value;
            moveTarget.gameObject.SetActive(value);
        }
    }
    Camera MainCam;
    Ray ray;
    RaycastHit hitinfo;
    RangeCharacterSelector Selector;
    float MoveDistance = -1.0f;
    // Start is called before the first frame update
    void Start()
    {
        MainCam = GetComponent<Camera>();
        Selector = GetComponent<RangeCharacterSelector>();
        targetPointMateral = moveTarget.GetComponent<Renderer>().material;
    }

    public bool isUI
    {
        get
        {//判断是否点击的是UI
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count != 0;
        }
    }

    //刷新鼠标即时距离显示，以及返回距离值，不可达返回负值
    float CalculateRoute(CharacterScript character, Vector3 point)
    {
        float ret = 0.0f;
        Vector3[] vectors = null;
        isNeedMousePnlOn = true;
        isNeedTargetPointOn = true;
        moveTarget.Set(point);
        mousePnl.FollowMouse(Input.mousePosition);
        NavMeshPath path = new NavMeshPath();
        if (character.NavAgent.CalculatePath(point, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            vectors = new Vector3[path.corners.Length + 2];
            int i = 0;
            vectors[0] = character.transform.position + LineOffset;
            for (i = 0; i < path.corners.Length; i++)
            {
                vectors[i + 1] = path.corners[i] + LineOffset;
                ret += (vectors[i + 1] - vectors[i]).magnitude;
            }
            vectors[i + 1] = point + LineOffset;
            ret += (vectors[i + 1] - vectors[i]).magnitude;
            ret /= OnGame.WorldScale;
            mousePnl.SetStr2(ret.ToString("f2") + "米");
            if (OnScene.isTimerModeOn)
            {
                isNeedRouteOn = true;
                MovePreviewRoute.positionCount = vectors.Length;
                MovePreviewRoute.SetPositions(vectors);
                float APcost = character.data.WalkAPCost(ret);
                Buff buff = null;
                if (character.isHaveBuff("禁足", out buff))
                    APcost = 9999.0f;
                else if (character.isHaveBuff("荆棘丛生", out buff))
                    APcost /= (100 - buff.value) / 100;
                AP_Panel.SetPreview(APcost);
                if (character.data.APCanDo(APcost))
                {
                    if (character.data.AP - APcost > 2.0f)
                    {
                        targetPointMateral.SetColor("_TintColor", GreenTargetColor);
                        MovePreviewRoute.startColor = GreenRouteColor;
                        MovePreviewRoute.endColor = GreenRouteColor;
                    }
                    else
                    {
                        targetPointMateral.SetColor("_TintColor", YellowTargetColor);
                        MovePreviewRoute.startColor = YellowRouteColor;
                        MovePreviewRoute.endColor = YellowRouteColor;
                    }
                    mousePnl.SetStr1("左键移动");
                }
                else
                {
                    targetPointMateral.SetColor("_TintColor", RedTargetColor);
                    MovePreviewRoute.startColor = RedRouteColor;
                    MovePreviewRoute.endColor = RedRouteColor;
                    mousePnl.SetStr1("行动点不足");
                    ret = -1.0f;
                }
            }
            else
            {
                targetPointMateral.SetColor("_TintColor",GreenTargetColor);
                mousePnl.SetStr1("右键移动");
            }
        }
        else
        {
            mousePnl.SetStr1("右键移动");
            mousePnl.SetStr2("不可达");
            ret = -1.0f;
            targetPointMateral.SetColor("_TintColor", RedTargetColor);
        }
        return ret;
    }

    void OnClickObject(ClickType clickType, Collider collider)
    {
        StaticBlock staticBlock = null;
        switch (collider.tag)
        {
            case "StaticBlock":
                staticBlock = collider.GetComponent<StaticBlock>();
                if (staticBlock == null)
                {
                    staticBlock = collider.transform.parent.GetComponent<StaticBlock>();
                }
                if (staticBlock != null)
                {
                    if (Selector.isListening)
                    {
                        Selector.StaticBlockListener(clickType, staticBlock,hitinfo.point);
                    }
                    else
                    {
                        switch (clickType)
                        {
                            case ClickType.Left:
                                if (OnScene.isTimerModeOn && OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    if (MoveDistance > 0)
                                    {
                                        OnScene.onSelect.character.SetMove(hitinfo.point, MoveDistance);
                                    }
                                    return;
                                }
                                else
                                {
                                    OnScene.bottomPnl.SetInfo(staticBlock.tile.info.name, "地面", staticBlock.tile.info.log);
                                    OnScene.onSelect.SetSelect(staticBlock);
                                    return;
                                }
                            case ClickType.Right:
                                if (!OnScene.isTimerModeOn && OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    if (MoveDistance > 0)
                                    {
                                        OnScene.onSelect.character.SetMove(hitinfo.point, MoveDistance);
                                    }
                                    return;
                                }
                                else
                                {
                                    OnScene.bottomPnl.SetInfo(staticBlock.tile.info.name, "地面", staticBlock.tile.info.log);
                                    OnScene.onSelect.SetSelect(staticBlock);
                                    return;
                                }
                            case ClickType.Nul:
                                if (OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    MoveDistance = CalculateRoute(OnScene.onSelect.character, hitinfo.point);
                                }
                                return;
                            default:
                                return;
                        }
                    }
                }
                return;
            case "MapObject":
                staticBlock = collider.transform.parent.GetComponent<StaticBlock>();
                if (staticBlock != null)
                {
                    if (Selector.isListening)
                    {
                        Selector.MapObjectListener(clickType, staticBlock,hitinfo.point);
                    }
                    else
                    {
                        switch (clickType)
                        {
                            case ClickType.Left:
                                if (OnScene.isTimerModeOn && OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    if (MoveDistance > 0)
                                    {
                                        OnScene.onSelect.character.SetMove(hitinfo.point, MoveDistance);
                                    }
                                    return;
                                }
                                else
                                {
                                    OnScene.bottomPnl.SetInfo(staticBlock.tile.mapObj.name, "场景物体", staticBlock.tile.mapObj.log);
                                    OnScene.onSelect.SetSelect(staticBlock.tile.mapObj);
                                    return;
                                }
                            case ClickType.Right:
                                if (!OnScene.isTimerModeOn && OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    if (MoveDistance > 0)
                                    {
                                        OnScene.onSelect.character.SetMove(hitinfo.point, MoveDistance);
                                    }
                                    return;
                                }
                                else
                                {
                                    OnScene.bottomPnl.SetInfo(staticBlock.tile.mapObj.name, "场景物体", staticBlock.tile.mapObj.log);
                                    OnScene.onSelect.SetSelect(staticBlock.tile.mapObj);
                                    return;
                                }
                            case ClickType.Nul:
                                if (OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    MoveDistance = CalculateRoute(OnScene.onSelect.character, hitinfo.point);
                                }
                                return;
                            default:
                                return;
                        }
                    }
                }
                return;
            case "SummonObj":
                SummonObjectBase summonObject = collider.GetComponent<SummonObjectBase>();
                if (summonObject != null)
                {
                    if (Selector.isListening)
                    {
                        Selector.SummonObjListener(clickType, summonObject, hitinfo.point);
                    }
                    else
                    {
                        switch (clickType)
                        {
                            case ClickType.Left:
                                if (OnScene.isTimerModeOn && OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    if (MoveDistance > 0)
                                    {
                                        OnScene.onSelect.character.SetMove(hitinfo.point, MoveDistance);
                                    }
                                    return;
                                }
                                else
                                {
                                    OnScene.bottomPnl.SetInfo(summonObject.Name, "召唤物", summonObject.Log);
                                    OnScene.onSelect.SetSelect(summonObject);
                                    return;
                                }
                            case ClickType.Right:
                                if (OnScene.userPanel.mode == UserPanel.UserPanelMode.Character && Input.GetKey(KeyCode.LeftShift))
                                {
                                    summonObject.OnRightClick(OnScene.onSelect.character);
                                    return;
                                }
                                else if (!OnScene.isTimerModeOn && OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    if (MoveDistance > 0)
                                    {
                                        OnScene.onSelect.character.SetMove(hitinfo.point, MoveDistance);
                                    }
                                    return;
                                }
                                else
                                {
                                    OnScene.bottomPnl.SetInfo(summonObject.Name, "召唤物", summonObject.Log);
                                    OnScene.onSelect.SetSelect(summonObject);
                                    return;
                                }
                            case ClickType.Nul:
                                if (OnScene.userPanel.mode == UserPanel.UserPanelMode.Character)
                                {
                                    MoveDistance = CalculateRoute(OnScene.onSelect.character, hitinfo.point);
                                    if (Input.GetKey(KeyCode.LeftShift))
                                    {
                                        mousePnl.SetStr1(summonObject.Name);
                                        mousePnl.SetStr2("右键使用");
                                    }
                                }
                                return;
                            default:
                                return;
                        }
                    }
                }
                return;
            case "Character":
                CharacterScript characterScript = collider.transform.GetComponent<CharacterScript>();
                if (characterScript != null)
                {
                    if (Selector.isListening)
                    {
                        Selector.CharacterListener(clickType, characterScript);
                    }
                    else
                    {
                        switch (clickType)
                        {
                            case ClickType.Left:
                                OnScene.onSelect.SetSelect(characterScript);
                                OnScene.bottomPnl.SetInfo(characterScript);
                                return;
                            case ClickType.Right:
                                return;
                            case ClickType.Nul:
                                MoveDistance = -1;
                                return;
                            default:
                                return;
                        }
                    }
                }
                return;
            default:
                return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ToMenu"))
        {
            TitleCanvas.SetActive(true);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Sound.PlayClick();
        }
        isNeedMousePnlOn = false;
        isNeedRouteOn = false;
        isNeedTargetPointOn = false;
        if (OnScene.isPlayMode == false)
        {
            //这地方这么写肯定不行，过后再优化
            OnScene.onSelect.SetSelectNull();
            return;
        }
        if (Selector.isListening && Selector.type == RangeCharacterSelector.SelectorType.CanvasRect)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Selector.RectCatchOnMouseDown(Input.mousePosition);
            }
            if (!Selector.isRectCatching)
                return;
            if (Input.GetMouseButton(0))
            {
                Selector.RectCatchDuringMouseDown(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                Selector.RectCatchOnMouseUp(Input.mousePosition);
            }
            return;
        }
        ClickType click = Input.GetMouseButtonDown(0) ? ClickType.Left : ClickType.Nul;
        click = Input.GetMouseButtonDown(1) ? ClickType.Right : click;
        if (isUI)//如果点到了UI上
        {
            if (click != ClickType.Nul)
            {
                if (Selector.isListening)
                {
                    Selector.CancelSelect();
                }
            }
        }
        else
        {
            ray = MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitinfo))
            {
                OnClickObject(click, hitinfo.collider);
            }
            else
            {
                //
            }
        }
        if (isNeedRouteOn != isRouteOn)
            isRouteVisable = isNeedRouteOn;
        if (isNeedMousePnlOn != isMousePnlOn)
            isMousePnlVisable = isNeedMousePnlOn;
        if (isNeedTargetPointOn != isTargetPointOn)
            isTargetPointVisable = isNeedTargetPointOn;

    }
}
