using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveJoyStick : MonoBehaviour
{
    #region  欄位
    [Header("搖桿")]
    public GameObject joyStick;
    [Header("搖桿背景圖範圍"), Tooltip("JoyBackGround背景圖")]
    public GameObject joyBG;
    [Header("搖桿半徑")]
    public float jyRadiu;
    //搖桿離背景圖中心的距離
    public Vector2 direction;
    //搖桿原點
    public Vector2 startPos = Vector2.zero;
    //目前搖桿位置
    Vector2 joyStickV2;
    //位移後 搖桿位置
    Vector2 joyStickV2Move;

    [Tooltip("玩家自己")]
    public Rigidbody playerA;
    [Header("角色動畫")]
    public Animation anim;

    //玩家自己的攝影機 視角
    public Camera playerAC;

    //是否離開地面
    bool vacate = false;
    #endregion

    void Start()
    {
        joyStick = GetComponent<GameObject>();
        joyBG = GetComponent<GameObject>();
        anim = GetComponent<Animation>();
        playerAC = GetComponent<Camera>();

        joyStick.transform.position = startPos;
    }

    void Update()
    {


    }

    void FixedUpdate()
    {
        //偵測是否離開地面，地面Tag要調整
        if (Physics.Raycast(playerA.transform.position, Vector3.down, 0.1f, 1 >> 4))
        {
            vacate = false;
        }


    }

    #region 方法
    /// <summary>
    ///角色位移
    /// </summary>
    private void PlayerRotate()
    {
        //位移到一半，不能移動，放開 才位移完，在移動。要調整


    }


    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        joyStick.transform.position = Input.GetTouch(0).position;
        //搖桿與原點 的向量
        joyStickV2 = (Vector2)joyStick.transform.position - startPos;
        //紀錄 位移後 位置
        joyStickV2Move= Input.GetTouch(0).position;

        //右腳
        if (joyStick.transform.position.x > 0)
        {
            if (joyStickV2.x / jyRadiu < 0.5f)
            {
                if (joyStickV2.y / jyRadiu > 0.5f)
                {
                    //1點鐘方向動畫
                    if (joyStick.transform.position.y > 0)
                    {

                        PlayerRotate();
                    }
                    //5點鐘方向動畫
                    else
                    {

                        PlayerRotate();
                    }
                }
            }
            //2點鐘方向動畫
            else if (joyStick.transform.position.y > 0 )
            {

                        PlayerRotate();
            }
            //4點鐘方向動畫
            else if (joyStick.transform.position.y < 0)
            {

                        PlayerRotate();
            }
        }
        //左腳
        if (joyStick.transform.position.x < 0)
        {
            if (joyStickV2.x / jyRadiu < 0.5f)
            {
                if (joyStickV2.y / jyRadiu > 0.5f)
                {
                    //11點鐘方向動畫
                    if (joyStick.transform.position.y > 0)
                    {

                        PlayerRotate();
                    }
                    //7點鐘方向動畫
                    else
                    {

                        PlayerRotate();
                    }
                }
            }
            //10點鐘方向動畫
            else if (joyStick.transform.position.y > 0)
            {

                        PlayerRotate();
            }
            //8點鐘方向動畫
            else if (joyStick.transform.position.y < 0)
            {

                        PlayerRotate();
            }
        }
    }
    public void OnMove(PointerEventData eventData)
    {
        joyStick.transform.position = Input.GetTouch(0).position;
        joyStickV2 = (Vector2)joyStick.transform.position - startPos;
        if (joyStickV2.x > jyRadiu * 0.6 || joyStickV2.y > jyRadiu * 0.6)
        {
            //跑步方向同步攝影機
            playerA.velocity = joyStick.transform.position * (Vector2)playerAC.transform.forward * 1.5f;
        }
        else
        {
            //走路方向同步攝影機
            playerA.velocity = joyStick.transform.position * (Vector2)playerAC.transform.forward;
        }


    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //跳躍
        if (!vacate)
        {
            playerA.velocity = new Vector3(0, 2, 0);
        }
    }
}
