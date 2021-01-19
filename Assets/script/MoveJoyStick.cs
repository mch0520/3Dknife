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
    //位移後 要轉動的向量，以 位移後 搖桿位置 為原點
    Vector2 joyStickMV2;

    [Tooltip("玩家自己")]
    public Rigidbody playerA;
    [Header("角色動畫")]
    public Animation anim;
    [Header("角色速度"), Range(0.5f, 1.2f)]
    public float speed;
    //初始速度
    float startSpeed;
    //角色位移後再 轉動值
    private float rotatePlayA=0;
    //敵人的武器角度
    float armathB;

    //玩家自己的攝影機 視角
    public Camera playerAC;

    //是否離開地面
    bool vacate = false;

    //頓下的時間
    float squatTime;
    #endregion

    void Start()
    {
        joyStick = GetComponent<GameObject>();
        joyBG = GetComponent<GameObject>();
        anim = GetComponent<Animation>();
        playerAC = GetComponent<Camera>();
        playerA = GetComponent<Rigidbody>();

        joyStick.transform.position = startPos;
        startSpeed = speed;
    }

    void Update()
    {
        //頓下時間 開始計時，0.5秒後重置
        if (squatTime != 0)
        {
            float squatTimeB = Time.deltaTime;
            if (squatTimeB > 0.5f)
            {
                squatTime = 0;
                squatTimeB = 0;
            }
        }

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
    ///角色位移，到放開 才執行，要調整
    /// </summary>
    private void PlayerRotate()
    {
        //跨完動畫

        //位移到一半，不能移動，放開 才位移完，在移動。要調整
        joyStickMV2 = joyStick.GetComponent<RectTransform>().anchoredPosition - joyStickV2Move;

        #region Atan2的xy為0,返回正確的角度,而不是拋出被0除的異常
        if (joyStickMV2.x == 0 && joyStickMV2.y > 0)
        {
            rotatePlayA = 90;
        }
        else if (joyStickMV2.y < 0)
        {
            rotatePlayA = 270;
        }

        if (joyStickMV2.y == 0 && joyStickMV2.x >= 0)
        {
            rotatePlayA = 0;
        }
        else
        {
            rotatePlayA = 180;
        }
        #endregion

        #region 向量轉為角度
        if (joyStickMV2.x != 0 && joyStickMV2.y != 0)
        {
            //向量轉斜率Atan2() * 弧度轉角度 
            //弧度轉角度 為 常數 Rad2Deg =57.29578
            rotatePlayA = Mathf.Atan2(joyStickMV2.x, joyStickMV2.y) * Mathf.Rad2Deg;
        }
        #endregion

        #region 控制角度在0~360
        if (rotatePlayA < 0)
        {
            rotatePlayA += 360;
        }
        if (rotatePlayA > 360)
        {
            rotatePlayA -= 360;
        }
        #endregion

        //normalized=1,角色轉向
        playerA.transform.eulerAngles = new Vector3(0, rotatePlayA, 0);
        joyStickV2Move = Vector2.zero;
    }

    //跨步();
    #endregion

    #region 事件
    private void OnTriggerEnter(Collider other)
    {
        speed = 0;
        anim.Play("back");
        speed = startSpeed;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        joyStick.transform.position = Input.GetTouch(0).position;
        //搖桿與原點 的向量
        joyStickV2 = (Vector2)joyStick.transform.position - startPos;
        //紀錄 位移後 位置
        joyStickV2Move = Input.GetTouch(0).position;

        //格擋時，跨步擊潰攻擊
        if (speed == 0 && (180 - armathB) > 90 && (180 - armathB) < 270)
        {
            if (joyStickV2Move.x < 0 && joyStickV2Move.y > 0)
            {
                //跨步();
                rotatePlayA = 1;
            }
        }

        //站立
        if (squatTime < 0)
        {
            anim.Play("站立");
            squatTime = 0;
        }
        if (anim["蹲下"].normalizedSpeed == 1)
        {
            squatTime -= Time.deltaTime;
        }
        //蹲下
        if (squatTime > 0)
        {
            anim.Play("蹲下");
            squatTime = 0;
        }
        if (anim["蹲下"].normalizedSpeed == 0)
        {
            squatTime += Time.deltaTime;
        }

        if (speed > 0)
        {
            //右腳跨出去
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
                else if (joyStick.transform.position.y > 0)
                {

                    PlayerRotate();
                }
                //4點鐘方向動畫
                else if (joyStick.transform.position.y < 0)
                {

                    PlayerRotate();
                }
            }
            //左腳跨出去
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
    }
    public void OnMove(PointerEventData eventData)
    {
        //蹲下不拖動
        squatTime = 0;
        joyStick.transform.position = Input.GetTouch(0).position;
        joyStickV2 = (Vector2)joyStick.transform.position - startPos;
        if (joyStickV2.x > jyRadiu * 0.6 || joyStickV2.y > jyRadiu * 0.6)
        {
            //跑步方向同步攝影機
            playerA.velocity = joyStick.transform.position * (Vector2)playerAC.transform.forward * 1.5f;
            anim.Play("run");
        }
        else if (squatTime == 0)
        {
            //走路方向同步攝影機
            playerA.velocity = joyStick.transform.position * (Vector2)playerAC.transform.forward;
            anim.Play("walk");
        }
        else
        {
            //走路
            playerA.velocity = joyStick.transform.position * (Vector2)playerAC.transform.forward*0.7f;
            anim.Play("walk");

        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //跳躍
        if (!vacate)
        {
            playerA.velocity = new Vector3(0, 2, 0);
        }

        if (rotatePlayA>0)
        {
            joyStickMV2 = joyStickV2 - joyStickV2Move;
            playerA.rotation.eulerAngles=new Vector3(0, rotatePlayA, 0);
            speed = startSpeed;
            rotatePlayA = 0;
        }
        else if (speed > startSpeed)
        {
            anim.Play("jump");
            speed = startSpeed;
        }

    }
    #endregion
}
