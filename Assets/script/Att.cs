using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Att : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject jsb;      //搖桿joystick,背景background
    public GameObject js;       //搖桿joystick
    [Tooltip("搖桿最大半徑以像素為單位")]
    public float jsr;
    private RectTransform selfTransform;//
    bool isTouched = false;     //是否觸摸虛擬搖桿
    private Vector2 originPosition;//虛擬搖桿初始位置
    private Vector2 touchedAxis; //搖桿移動方向

  
    public Vector2 TouchedAxis
    {
        get
        {
            if (touchedAxis.magnitude < jsr)
                return touchedAxis.normalized / jsr;
            return touchedAxis.normalized;
        }
    }
    /// <summary>
    /// 定义触摸开始事件委托
    /// </summary>
    public delegate void JoyStickTouchBegin(Vector2 vec);
    /// <summary>
    /// 定义触摸过程事件委托
    /// </summary>
    /// <param name="vec">虚拟摇杆的移动方向</param>
    public delegate void JoyStickTouchMove(Vector2 vec);
    /// <summary>
    /// 定义触摸结束事件委托
    /// </summary>
    public delegate void JoyStickTouchEnd();
    /// <summary>
    /// 注册触摸开始事件
    /// </summary>
    public event JoyStickTouchBegin OnJoyStickTouchBegin;
    /// <summary>
    /// 注册触摸过程事件
    /// </summary>
    public event JoyStickTouchMove OnJoyStickTouchMove;
    /// <summary>
    /// 注册触摸结束事件
    /// </summary>
    public event JoyStickTouchEnd OnJoyStickTouchEnd;
    void Start()
    {
        //初始化虚拟摇杆的默认方向
        selfTransform = this.GetComponent<RectTransform>();
        originPosition = selfTransform.anchoredPosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isTouched = true;
        touchedAxis = GetJoyStickAxis(eventData);
        if (this.OnJoyStickTouchBegin != null)
            this.OnJoyStickTouchBegin(TouchedAxis);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isTouched = false;
        selfTransform.anchoredPosition = originPosition;
        touchedAxis = Vector2.zero;
        if (OnJoyStickTouchEnd != null)
            OnJoyStickTouchEnd();
    }
    public void OnDrag(PointerEventData eventData)
    {
        touchedAxis = GetJoyStickAxis(eventData);
        if (OnJoyStickTouchMove != null)
            OnJoyStickTouchMove(TouchedAxis);
    }
    void Update()
    {
        //当虚拟摇杆移动到最大半径时摇杆无法拖动
        //为了确保被控制物体可以继续移动
        //在这里手动触发OnJoyStickTouchMove事件
        if (isTouched && touchedAxis.magnitude >= jsr)
        {
            if (OnJoyStickTouchMove != null)
                OnJoyStickTouchMove(TouchedAxis);
        }
        //松开虚拟摇杆后让虚拟摇杆回到默认位置
        if (selfTransform.anchoredPosition.magnitude > originPosition.magnitude)
            selfTransform.anchoredPosition -= TouchedAxis * Time.deltaTime * 5.0f;      //摇杆重置所诉JoyStickResetSpeed=5.0f
    }
    /// <summary>
    /// 返回虚拟摇杆的偏移量
    /// </summary>
    /// <returns>The joy stick axis.</returns>
    /// <param name="eventData">Event data.</param>
    private Vector2 GetJoyStickAxis(PointerEventData eventData)
    {
        //获取手指位置的世界坐标
        Vector3 worldPosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(selfTransform,
                 eventData.position, eventData.pressEventCamera, out worldPosition))
            selfTransform.position = worldPosition;
        //获取摇杆的偏移量
        Vector2 touchAxis = selfTransform.anchoredPosition - originPosition;
        //摇杆偏移量限制
        if (touchAxis.magnitude >= jsr)
        {
            touchAxis = touchAxis.normalized * jsr;
            selfTransform.anchoredPosition = touchAxis;
        }
        return touchAxis;
    }
}
