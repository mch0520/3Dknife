using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://k79k06k02k.pixnet.net/blog/post/114531737?pixfrom=related

public class JoyStick : MonoBehaviour
{
    [Header("搖桿")]
    public GameObject joyStick;
    [Header("搖桿背景圖範圍")]
    public GameObject joyBG;
    [Header("搖桿半徑")]
    public float jyRadiu;
    //搖桿離背景圖中心的距離
    public Vector2 direction;
    //搖桿原點
    public Vector2 startPos;
    //上一禎的點
    public Vector2 oldPos;
    //當前禎的點
    public Vector2 newPos;
    //當前禎跟前一禎的座標差
    public Vector2 no;

    //
    public GameObject player1;
    //武器活動參數
    public float armath;
    //武器本體
    public GameObject arm;
    [Header("武器活動第三、四象限的變數")]
    public float lrrl;
    //左前右後模式
    public float lrmode=0;


    //是否觸摸虛擬搖桿
    bool isTouched = false;

    //動畫
    public Animation anim;


    public void Start()
    {
        joyStick = GetComponent<GameObject>();
        joyBG = GetComponent<GameObject>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouched = true;
        startPos = Input.GetTouch(0).position;
        joyStick.transform.position = startPos;
        //播放拔刀動畫
        //
    }
    public void OnMove(PointerEventData eventData)
    {
        //搖桿對背景圖中心的距離
        Touch touch = Input.GetTouch(0);
        direction = touch.position - startPos;
        //在範圍內移動
        if (direction.x<jyRadiu& direction.y < jyRadiu)
        {
            //不是格黨才能移動搖桿(碰撞體碰撞)
            if (arm.gameObject.GetComponent<Collider>().isTrigger)
            {
                joyStick.transform.position = touch.position;
                //play knifeSound
            }
            //前臂動畫的即時轉向
            //持續更新當前禎
            newPos = joyStick.transform.position;
            //當前禎減前一禎的座標算向量
            no = newPos - oldPos;
            //當前禎變前一禎，前一禎取得當前禎的值
            oldPos = newPos;
            //斜率轉成弧度 弧度轉角度
            armath = Mathf.Atan2(no.y,no.x)*Mathf.Rad2Deg;
            if(armath<0)
            {
                armath += 360;
            }

            {   
                #region 武器揮動的細節
                //武器旋轉的角度armath
                //武器旋轉的角度轉成0~1給前臂以下的動畫
                //右前左後rl，死角第四象限(換手)
                if (armath < 270 - lrrl & armath > 0)
                {
                    //換手
                    if (lrmode ==1)
                    {
                        anim.Play("lrrl");
                        lrmode = 0;
                    }
                    //持續非左前右後模式
                    if (lrmode ==0 & anim["lrrl"].normalizedTime == 1)
                    {
                        //arm.transform.right = Quaternion(arm.transform.position.x, arm.transform.position.y, armath);
                        arm.transform.eulerAngles = new Vector3(arm.transform.position.x, arm.transform.position.y, armath);
                        anim["rl"].normalizedTime = armath;
                    }
                }
                //左前右後lr,死角第三象限(換手)
                else if (armath > 270 + lrrl & armath < 180 & lrmode==1)
                {
                    //換手
                    if (lrmode ==0)
                    {
                        anim.Play("rllr");
                        lrmode = 1;
                    }
                    //持續左前右後模式
                    if (lrmode == 1 & anim["rllr"].normalizedTime ==1)
                    {
                        //arm.transform.right = Quaternion( eulerAngle[arm.transform.position.x, arm.transform.position.y, armath]);
                        arm.transform.eulerAngles = new Vector3(arm.transform.position.x,arm.transform.position.y,armath);
                        anim["lr"].normalizedTime = armath;
                    }
                }//死角轉正，未測試空條件是否可執行
                else if (armath > 270 - lrrl & armath < 270 + lrrl)
                {
                    arm.transform.right = Vector2.zero;

                }


            }
            #region
        }



        //播放揮刀動畫
        //

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        joyStick.transform.position = startPos;
        isTouched = false;
        //播放收刀動畫
        //
    }


}
