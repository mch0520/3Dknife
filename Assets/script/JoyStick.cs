using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/** https://k79k06k02k.pixnet.net/blog/post/114531737?pixfrom=related **/
//武器跟角色轉向未實行

public class JoyStick : MonoBehaviour
{
    #region 欄位
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
    //上一禎的點
    public Vector2 oldV2;
    //當前禎的點
    public Vector2 newV2;
    //當前禎跟前一禎的座標差
    public Vector2 noV2;

    //上一禎的角度
    public float oldDeg;
    //當前禎的角度
    public float newDeg;


    //武器活動參數
    public float armath;
    //碰撞時 我的武器角度
    public float armathA;
    //敵人的武器角度
    public float armathB;
    //武器本體
    public GameObject arm;
    //武器碰撞器
    private Collider armRigi;
    [Header("武器活動第三、四象限的角度變數")]
    public float quadrant;
    //左前右後模式
    public int lrmode = 0;


    //是否觸摸虛擬搖桿
    bool isTouched = false;

    //武器是否互相碰撞
    public bool parry = false;
    //碰撞時 上一禎的點
    public Vector2 oldV2Att;
    //碰撞時 當前禎的點
    public Vector2 newV2Att;
    //碰撞時 當前禎跟前一禎的座標差
    public Vector2 noV2Att;
    [Header("碰撞音效")]
    public AudioSource collideSound;

    //武器動畫
    public Animation anim;
    //攻擊進入點
    public Vector3 enterV3;
    //攻擊離開點
    public Vector3 exitV3;
    [Header("攻擊力")]
    float power;
    
    #endregion

    public void Start()
    {
        joyStick = GetComponent<GameObject>();
        joyBG = GetComponent<GameObject>();
        armRigi = arm.GetComponent<Collider>();
        anim = GetComponent<Animation>();
        collideSound = GetComponent<AudioSource>();
        //重置搖桿的位置
        joyStick.transform.position = startPos;
    }
    private void Update()
    {
        //武器互撞事件
        if (armRigi.isTrigger)
        {
            //停住搖桿
            isTouched = false;
            //觸發格黨事件
            Parry();
            parry = true;
        }

    }

    #region 事件

    #region 攻擊處理,要調整
    private void Attack()
    {
        //計算(攻擊長度/30cm)*攻擊力，要調整
        power = (enterV3.x - exitV3.x)* (enterV3.y - exitV3.y)* (enterV3.z - exitV3.z)/30;
    }
    /// <summary>
    /// 打人的進入點
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collision collision)
    {
        Vector3 ClosestPoint = armRigi.ClosestPoint(enterV3);
    }
    /// <summary>
    /// 打人的離開點
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collision collision)
    {
        Vector3 ClosestPoint = armRigi.ClosestPoint(exitV3);
    }
    #endregion

    /// <summary>
    /// 格黨 touch的每一楨向量 轉 角度
    /// </summary>
    private float Parry()
    {
        //搖桿 跟 碰撞時停住的動畫點 同步
        //搖桿點/最大半徑=動畫點
        //搖桿點=動畫點*最大半徑
        joyStick.transform.position = new Vector2(anim["x"].normalizedTime * jyRadiu, anim["y"].normalizedTime * jyRadiu);
        collideSound.Play();

        #region 格黨時 每一楨的向量noV2Att
        Touch touch = Input.GetTouch(0);
        //碰撞時 兩座標的向量
        noV2Att = touch.position - (Vector2)joyStick.transform.position;
        #endregion

        #region Atan2的xy為0,返回正確的角度,而不是拋出被0除的異常
        if (noV2Att.x == 0 && noV2Att.y > 0)
        {
            armathA = 90;
        }
        else if (noV2Att.y < 0)
        {
            armathA = 270;
        }

        if (noV2Att.y == 0 && noV2Att.x >= 0)
        {
            armathA = 0;
        }
        else
        {
            armathA = 180;
        }
        #endregion

        #region 向量轉為角度
        if (noV2Att.x != 0 && noV2Att.y != 0)
        {
            //向量轉斜率Atan2() * 弧度轉角度 
            //弧度轉角度 為 常數 Rad2Deg =57.29578
            armathA = Mathf.Atan2(noV2Att.x, noV2Att.y) * Mathf.Rad2Deg;
        }
        #endregion

        #region 控制角度在0~360
        if (armathA < 0)
        {
            armathA += 360;
        }
        if (armathA > 360)
        {
            armathA -= 360;
        }
        #endregion
        return armathA;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouched = true;
        joyStick.transform.position = Input.GetTouch(0).position;
        //播放拔刀動畫
        //根據點擊區域撥放不同種動畫，要調整
    }
    public void OnMove(PointerEventData eventData)
    {
        //搖桿對背景圖中心的距離
        Touch touch = Input.GetTouch(0);
        direction = touch.position - startPos;
        //在正方形範圍內移動
        if (direction.x < jyRadiu & direction.y < jyRadiu)
        {
            #region 離開格黨的範圍
            //武器互撞後,對敵人武器的方向  不能移動搖桿(碰撞體碰撞)
            //180-對方的角度 為 看到的相對角度
            //可切入 相對角度再加95度以外
            if (armathA > (180 - armathB) + 95 & armathA < (180 - armathB) - 95)
            {
                //允許觸摸搖桿
                isTouched = true;
                parry = false;
            }
            #endregion

            //可以觸摸搖桿
            if (isTouched)
            {
                joyStick.transform.position = touch.position;

                #region 搖桿 每一楨的向量noV2
                //更新當前楨
                newV2 = joyStick.transform.position;
                //兩座標的向量
                noV2 = newV2 - oldV2;
                //當前楨變上一楨,上一楨 設定為 當前楨
                oldV2 = newV2;
                #endregion

                #region 上下左右揮舞動畫
                //左右揮舞動畫
                anim.Play("x");
                if (newV2.x / jyRadiu - anim["x"].normalizedTime < 0)            anim["x"].speed = -1; 
                else if (newV2.x / jyRadiu - anim["x"].normalizedTime > 0)       anim["x"].speed = 1; 
                else  anim["x"].speed = 0; 

                //上下揮舞動畫
                anim.Play("y");
                if (newV2.y / jyRadiu - anim["y"].normalizedTime < 0)            anim["y"].speed = -1; 
                else if (newV2.y / jyRadiu - anim["y"].normalizedTime > 0)       anim["y"].speed = 1; 
                else  anim["y"].speed = 0;
                #endregion

                #region Atan2的xy為0,返回正確的角度,而不是拋出被0除的異常
                if (noV2.x == 0 && noV2.y > 0)
                {
                    armath = 90;
                }
                else if (noV2.y < 0)
                {
                    armath = 270;
                }

                if (noV2.y == 0 && noV2.x >= 0)
                {
                    armath = 0;
                }
                else
                {
                    armath = 180;
                }
                #endregion

                #region 向量轉為角度
                if (noV2.x != 0 && noV2.y != 0)
                {
                    //向量轉斜率Atan2() * 弧度轉角度 
                    //弧度轉角度 為 常數 Rad2Deg =57.29578
                    armath = Mathf.Atan2(noV2.x, noV2.y) * Mathf.Rad2Deg;
                }
                #endregion

                #region 控制角度在0~360
                if (armath < 0)
                {
                    armath += 360;
                }
                if (armath > 360)
                {
                    armath -= 360;
                }
                #endregion

                #region 當前楨角度-上一楨角度算 順時針轉 或 逆時針轉
                //更新當前楨
                newDeg = armath;
                //當前楨變上一楨,上一楨 設定為 當前楨
                oldDeg = newDeg;

                if (newDeg > oldDeg)
                {
                    anim["rl"].speed = 1;
                    anim["lr"].speed = 1;
                }
                if (newDeg < oldDeg)
                {
                    anim["rl"].speed = -1;
                    anim["lr"].speed = -1;
                }
                #endregion

                #region 前臂動畫，要測試
                //武器旋轉的角度armath
                //武器旋轉的角度轉成0~1給前臂以下的動畫
                #region 右前左後rl，死角第四象限(換手)
                if (armath < 270 - quadrant && armath > 0)
                {
                    //換手
                    if (lrmode == 1)
                    {
                        anim.Play("lrrl");
                        lrmode = 0;
                    }

                    lrmode = 0;

                    //持續非左前右後模式
                    if (lrmode == 0 && (anim["lrrl"].normalizedTime == 0 || anim["lrrl"].normalizedTime == 1))
                    {
                        //arm.transform.right = Quaternion(arm.transform.position.x, arm.transform.position.y, armath);
                        //武器旋轉 跟 角色同步
                        arm.transform.eulerAngles = new Vector3(arm.transform.position.x, arm.transform.position.y, armath);
                        //
                        anim["rl"].speed = 1;
                        anim.Play("rl");

                        if (anim["rl"].normalizedTime == armath / 360)
                        {
                            anim["rl"].speed = 0;
                        }
                    }
                }
                #endregion

                #region 左前右後lr,死角第三象限(換手)
                else if (armath > 270 + quadrant && armath < 180)
                {
                    //換手
                    if (lrmode == 0)
                    {
                        anim.Play("rllr");
                        lrmode = 1;
                    }

                    //持續左前右後模式
                    if (lrmode == 1 && (anim["rllr"].normalizedTime == 0 || anim["rllr"].normalizedTime == 1))
                    {
                        //arm.transform.right = Quaternion( eulerAngle[arm.transform.position.x, arm.transform.position.y, armath]);
                        //武器旋轉 跟 角色同步
                        arm.transform.eulerAngles = new Vector3(arm.transform.position.x, arm.transform.position.y, armath);
                        //
                        anim["lr"].speed = 1;
                        anim.Play("lr");

                        if (anim["lr"].normalizedTime == armath / 360)
                        {
                            anim["lr"].speed = 0;
                        }
                    }
                }
                #endregion
                //死角轉正，未測試else是否可執行
                else
                {
                    //武器 與 角色同向
                    arm.transform.right = Vector2.zero;
                }
                #endregion
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        joyStick.transform.position = startPos;
        isTouched = false;

        //播放收刀動畫
        //
    }
    #endregion

}
