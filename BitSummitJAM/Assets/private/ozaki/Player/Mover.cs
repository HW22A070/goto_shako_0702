using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundMaskNumber;


    [SerializeField]
    [Tooltip("動く速さ")]
    private float _speed;

    [SerializeField]
    [Tooltip("空中機動力")]
    private float skymove;

    [SerializeField]
    [Tooltip("加速度")]
    private float acceleration;

    [SerializeField]
    [Tooltip("最高速度")]
    private float _maxSpeed;

    [SerializeField]
    [Tooltip("空中最高速度")]
    private float _skyMaxSpeed;

    [SerializeField]
    [Tooltip("地面についているか")]
    private bool Ground;

    /// <summary>
    /// 何秒地面にいるか測定用
    /// </summary>
    private float GroundTime;

    [SerializeField]
    private float punch;

    private float horizontal;
    [SerializeField]
    [Tooltip("パンチジャンプ1")]
    private float punchone;
    [SerializeField]
    [Tooltip("パンチジャンプ2")]
    private float punchtwo;
    [SerializeField]
    [Tooltip("パンチジャンプ3")]
    private float punchthree;


    private Vector3 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        //ポジション更新
        playerPos = transform.position;

        //グラウンドにいる状態で初期化
        Ground = true;

        //地面のRaycast用のマスクを取得
        LayerMask mask = 1 << groundMaskNumber;
    }

    private void Update()
    {
        //壁に触れていたら
        if (CheckWall_Right() && _speed > 0)
        {
            _speed = 0;

            punch = 0;
        }
        if (CheckWall_Left() && _speed < 0)
        {
            _speed = 0;

            punch = 0;
        }

        //LBボタンが押されたら
        if (Input.GetKeyUp("joystick button 4"))
        {
            //パンチを取得
            int hoge = GetComponent<PlayerPunch>().punch;

            //Lスティック入力を取得
            horizontal = -Input.GetAxis("Horizontal");

            if (hoge == 1)
            {
                punch = punchone * horizontal;
            }
            else if (hoge == 2)
            {
                punch = punchtwo * horizontal;
            }
            else if (hoge == 3)
            {
                punch = punchthree * horizontal;
            }

        }

        //地面に近いかの確認
        if (Physics2D.Raycast(playerPos, -transform.up, transform.localScale.y,groundMaskNumber))
        {
            //近い状態
            Ground = true;            

            GroundTime += Time.deltaTime;

            if (GroundTime > 0.1f)
            {
                punch = 0;

                GroundTime = 0;
            }
        }
        else
        {
            //離れている状態
            Ground = false;
        }

        //if (Ground == false)
        //{
        //    if (Physics2D.Raycast(playerPos, -transform.up, transform.localScale.y))
        //    {
        //        punch = 0;
        //    }
        //}

        //座標を更新
        playerPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //方向を決めるとき
        if (GetComponent<PlayerPunch>().Charge)
        {
            //動く速さを0に
            _speed = 0;
        }

        //地面から遠い時
        if (Ground == false)
        {
            if (Mathf.Abs(punch) >= _skyMaxSpeed - 5)
            {
                if (punch >= 0)
                {
                    punch = _skyMaxSpeed - 5;
                }
                if (punch <= 0)
                {
                    punch = -_skyMaxSpeed + 5;
                }
            }

            //動く速さ * パンチ移動スピード * 時間
            transform.position += transform.right * (_speed + punch) * Time.fixedDeltaTime;

            _speed += Input.GetAxisRaw("Horizontal") * skymove;

        }

        //地面に近い時
        else
        {
            if (Input.GetAxis("Horizontal") == 0)
            {
                if (_speed == 0)
                {
                    return;
                }
                if (_speed > 0)
                {
                    _speed -= 0.5f;
                }
                if (_speed < 0)
                {
                    _speed += 0.5f;
                }
                if(Mathf.Abs(_speed) <= 0.5)
                {
                    _speed = 0;
                }
            }

            //動く速さ * 時間
            transform.position += transform.right * _speed * Time.fixedDeltaTime;
        }

        //スピードに加速度を追加
        _speed += acceleration * Input.GetAxisRaw("Horizontal");

        //地面にいるときの最高速度を指定
        if (Mathf.Abs(_speed) >= _maxSpeed && Ground)
        {
            if (_speed > 0)
            {
                _speed = _maxSpeed;
            }
            else
            {
                _speed = -_maxSpeed;
            }
        }


        //地面から離れている時の最高速度を指定
        else if (Mathf.Abs(_speed) >= _skyMaxSpeed && Ground == false)
        {
            if (_speed > 0)
            {
                _speed = _skyMaxSpeed;
            }
            else
            {
                _speed = -_skyMaxSpeed;
            }
        }
    }

    private bool CheckWall_Right()
    {
        var rayTuple = CreateRay_Right();

        return
                Physics2D.Raycast(
                    rayTuple.Item1.origin,
                    rayTuple.Item1.direction,
                    this.transform.localScale.x * 0.6f,
                    groundMaskNumber) ||
                Physics2D.Raycast(
                    rayTuple.Item2.origin,
                    rayTuple.Item2.direction,
                    this.transform.localScale.x * 0.6f,
                    groundMaskNumber) ||
                Physics2D.Raycast(
                    rayTuple.Item3.origin,
                    rayTuple.Item3.direction,
                    this.transform.localScale.x * 0.6f,
                    groundMaskNumber);
    }

    private bool CheckWall_Left()
    {
        var rayTuple = CreateRay_Left();

        return
            Physics2D.Raycast(
                rayTuple.Item1.origin,
                rayTuple.Item1.direction,
                this.transform.localScale.x * 0.6f,
                groundMaskNumber) ||
            Physics2D.Raycast(
                rayTuple.Item2.origin,
                rayTuple.Item2.direction,
                this.transform.localScale.x * 0.6f,
                groundMaskNumber) ||
            Physics2D.Raycast(
                rayTuple.Item3.origin,
                rayTuple.Item3.direction,
                this.transform.localScale.x * 0.6f,
                groundMaskNumber);
    }

    public void KnockBack(bool hoge)
    {
        if (hoge)
        {
            this.transform.DOMoveX(2f, 0.5f);
        }
        else
        {
            this.transform.DOMoveX(-2f, 0.5f);
        }
    }



    /// <summary>
    /// 右向きのRayを生成する関数
    /// </summary>
    /// <returns>
    /// Item1 = 向かって上端が原点のRay
    /// Item2 = 向かって下端が原点のRay
    /// Item3 = 中心が原点のRay
    /// </returns>
    private (Ray, Ray, Ray) CreateRay_Right()
    {
        //右側のRayの原点を取得
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.y += this.transform.localScale.y * 0.4f;

        //左側のRayの原点を取得
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.y -= this.transform.localScale.y * 0.4f;

        //作成したRay2つを返す
        return (new Ray(rightOrigin, this.transform.right),
                new Ray(leftOrigin, this.transform.right),
                new Ray(this.transform.position, this.transform.right));
    }

    /// <summary>
    /// 左向きのRayを生成する関数
    /// </summary>
    /// <returns>
    /// Item1 = 向かって上端が原点のRay
    /// Item2 = 向かって下端が原点のRay
    /// Item3 = 中心が原点のRay
    /// </returns>
    private (Ray, Ray, Ray) CreateRay_Left()
    {
        //右側のRayの原点を取得
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.y += this.transform.localScale.y * 0.4f;

        //左側のRayの原点を取得
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.y -= this.transform.localScale.y * 0.4f;

        //作成したRay2つを返す
        return (new Ray(rightOrigin, -this.transform.right),
                new Ray(leftOrigin, -this.transform.right),
                new Ray(this.transform.position, -this.transform.right));
    }

    public float GraphicSender()
    {
        return _speed;
    }
}
