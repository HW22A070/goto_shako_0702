using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ジャンプを管理するクラス
/// </summary>
public class Jumper : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ジャンプの初速")]
    private float _v0;

    [SerializeField, Min(0.001f)]
    [Tooltip("ジャンプ力")]
    private float _jumpPow;

    [SerializeField]
    [Tooltip("重力")]
    private float _gravity;

    [SerializeField, Min(1)]
    [Tooltip("抵抗力")]
    private float _resistance;

    [SerializeField]
    [Tooltip("質量")]
    private float _mass;

    [SerializeField, Min(1)]
    [Tooltip("ジャンプ可能な回数")]
    private int _defaultJumpCount;

    /// <summary>
    /// Lスティックの入力取得用
    /// </summary>
    private float vertical;

    /// <summary>
    /// 縦方向の速度
    /// </summary>
    private float _verticalVelocity = 0;

    /// <summary>
    /// 終端速度
    /// </summary>
    private float _finalVelocity;

    /// <summary>
    /// 前回の接地判定の結果
    /// </summary>
    private bool _isLastGround = true;
    /// <summary>
    /// ジャンプを司る為のタイマー
    /// </summary>
    private float _jumpTimer = 0;

    /// <summary>
    /// 残りジャンプ可能回数
    /// </summary>
    private int _jumpCount;

    /// <summary>
    /// "Ground"レイヤーの番号
    /// </summary>
    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool jumppunch = false;
    public bool Jumppunch
    {
        get => jumppunch; set => jumppunch = value;
    }

    [SerializeField]
    private float punch;

    [SerializeField]
    [Tooltip("パンチジャンプ1")]
    private float punchone;
    [SerializeField]
    [Tooltip("パンチジャンプ2")]
    private float punchtwo;
    [SerializeField]
    [Tooltip("パンチジャンプ3")]
    private float punchthree;
    // Start is called before the first frame update
    private void Start()
    {
        //終端速度を取得
        UpdateFinalVelocity();

        //ジャンプ回数を初期化
        _jumpCount = _defaultJumpCount;

    }

    //GetKeyUpなど、FixedUpdateで都合がつかないものはこっちに書く
    private void Update()
    {
#if UNITY_EDITOR

        //デバッグ用に一定の高さを下回ったらログを出してプレイモード終了
        if (this.transform.position.y < -20)
        {
            UnityEditor.EditorApplication.isPlaying = false;

            Debug.Log("プレイヤーの高さが一定を下回った為、playモードを終了しました");
        }

#endif

        //ジャンプキーが離されたら
        if (Input.GetKeyUp("joystick button 2"))
        {
            _jumpTimer = 0;

            //もし空中で離されたらカウントを減らす
            if (!CheckGround())
                --_jumpCount;
        }

        if (Input.GetKeyUp("joystick button 4"))
        {
            int hoge = GetComponent<PlayerPunch>().punch;

            if (hoge == 1)
            {
                punch = punchone;
            }
            else if (hoge == 2)
            {
                punch = punchtwo;
            }
            else if (hoge == 3)
            {
                punch = punchthree;
            }

            vertical = -Input.GetAxis("Vertical");

            _jumpTimer = 0;

            //Jumppunch = true;

            //もし空中で離されたらカウントを減らす
            if (!CheckGround())
                --_jumpCount;

        }

        //接地状況が変化したか？(ただし、ジャンプによって空中に出たときは検証しない)
        if (_isLastGround != CheckGround())
        {
            //trueからfalseに切り替わったならジャンプ回数を1つ減らす
            //falseからtrueに切り替わったならデフォルトに戻す
            if (_isLastGround)
            {
                //もしジャンプで空中に出てないならばカウントを即座に１減らす
                if (_jumpTimer < Mathf.Epsilon)
                    --_jumpCount;
            }
            else
            {
                _jumpCount = _defaultJumpCount;
            }

            jumppunch = false;
        }

        //接地状況を更新
        _isLastGround = CheckGround();

        //地面に埋まっていたら上方向に補正をかける
        if (CheckBury())
            Correction_Up();

        //天井に埋まっていたら下方向に補正をかける
        if (OnCeiling())
            Correction_Down();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //接地しているかで分岐
        if (CheckGround())
        {
            //接地していたら縦方向の速度を0にする
            _verticalVelocity = 0;
        }
        else
        {
            //重力の適用
            ApplyingGravity();
        }

        //ジャンプ可能な時に、ジャンプキーが押下されたら
        if (Input.GetKey("joystick button 2") && JumpAble())
        {
            Jump();
        }

        //もし天井にぶつかったとき上方向に移動しようとしていたら
        if (OnCeiling() && _verticalVelocity > 0)
        {
            //速度を0に
            _verticalVelocity = 0;

            //ジャンプ用のタイマーに初速を入れて余計にジャンプしないように
            _jumpTimer = _v0;

            //ジャンプ回数を減らす
            --_jumpCount;
        }

        if (Jumppunch == true)
        {
            JumpPunch();
        }

        //動く
        Move();
    }

    /// <summary>
    /// 終端速度を更新する関数
    /// </summary>
    private void UpdateFinalVelocity()
    {
        //終端速度を定義
        _finalVelocity = _mass * _gravity / _resistance;
    }

    /// <summary>
    /// ジャンプをする関数
    /// </summary>
    private void Jump()
    {
        GetComponent<PlayerPunch>().PunchReset();

        _jumpTimer += Time.deltaTime / _jumpPow;

        //もし、タイマーが初速を超えてたら速度に触らない
        //※落下速度が上がるから
        if (_v0 < _jumpTimer || _verticalVelocity > _finalVelocity) return;

        //追加の速度を与える
        _verticalVelocity = _v0 - _jumpTimer;
    }
    /// <summary>
    /// パンチチャージ
    /// </summary>
    public void JumpPunch()
    {
        //パンチの初速にＬスティックの入力を加算
        float _v1 = _v0 + Mathf.Abs(vertical) * punch;

        _jumpTimer += Time.deltaTime / _jumpPow;

        //もし、タイマーが初速を超えてたら速度に触らない
        //※落下速度が上がるから
        if (_v1 < _jumpTimer || _verticalVelocity > _finalVelocity) return;

        //追加の速度を与える
        _verticalVelocity = _v1 - _jumpTimer;

    }

    /// <summary>
    /// 実際に動かす関数
    /// </summary>
    private void Move()
    {
        this.transform.position += this.transform.up * _verticalVelocity;
    }

    /// <summary>
    ///下方向に補正をかける関数
    /// </summary>
    private void Correction_Down()
    {
        //上向きのRay2つを取得
        var rayTupleData = CreateRay_Up();

        //右側から確認する
        RaycastHit2D hit = Physics2D.Raycast(
                                rayTupleData.Item1.origin,
                                rayTupleData.Item1.direction,
                                this.transform.localScale.y * 0.5f,
                                groundMask);

        //もし衝突物を検知出来たら
        if (hit.collider != null)
        {
            //当たった位置と現在位置の直線距離を取得
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //移動量を計算
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //現在の座標を取得
            var currentPosition = this.transform.position;

            //移動した後の座標を計算
            currentPosition.y -= moveVal;

            //座標を更新
            this.transform.position = currentPosition;
        }
        else
        {
            hit = Physics2D.Raycast(
                       rayTupleData.Item2.origin,
                       rayTupleData.Item2.direction,
                       this.transform.localScale.y * 0.6f,
                       groundMask);

            //もしここでもオブジェクトを取得出来なければここで終了
            if (hit.collider == null) return;

            //当たった位置と現在位置の直線距離を取得
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //移動量を計算
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //現在の座標を取得
            var currentPosition = this.transform.position;

            //移動した後の座標を計算
            currentPosition.y -= moveVal;

            //座標を更新
            this.transform.position = currentPosition;
        }
    }

    /// <summary>
    /// 上方向に補正を書ける関数
    /// </summary>
    private void Correction_Up()
    {
        //下向きのRay2つを取得
        var rayTupleData = CreateRay_Down();

        //右側から確認する
        RaycastHit2D hit = Physics2D.Raycast(
                                rayTupleData.Item1.origin,
                                rayTupleData.Item1.direction,
                                this.transform.localScale.y * 0.5f,
                                groundMask);

        //もし衝突物を検知出来たら
        if (hit.collider != null)
        {
            //当たった位置と現在位置の直線距離を取得
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //移動量を計算
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //現在の座標を取得
            var currentPosition = this.transform.position;

            //移動した後の座標を計算
            currentPosition.y += moveVal;

            //座標を更新
            this.transform.position = currentPosition;
        }
        else
        {
            hit = Physics2D.Raycast(
                       rayTupleData.Item2.origin,
                       rayTupleData.Item2.direction,
                       this.transform.localScale.y * 0.5f,
                       groundMask);

            //もしここでもオブジェクトを取得出来なければここで終了
            if (hit.collider == null) return;

            //当たった位置と現在位置の直線距離を取得
            var distance = Mathf.Abs(this.transform.position.y - hit.point.y);

            //移動量を計算
            var moveVal = Mathf.Abs(this.transform.localScale.y * 0.5f - distance);

            //現在の座標を取得
            var currentPosition = this.transform.position;

            //移動した後の座標を計算
            currentPosition.y += moveVal;

            //座標を更新
            this.transform.position = currentPosition;
        }
    }

    /// <summary>
    /// 重力を適用する関数
    /// </summary>
    private void ApplyingGravity()
    {
        //終端速度に達してなければ重力をかける
        if (_verticalVelocity < _finalVelocity * -1) return;

        _verticalVelocity -= _gravity;

    }

    /// <summary>
    /// 接地しているかを返す
    /// </summary>
    /// <returns>地面に接地している</returns>
    private bool CheckGround()
    {
        //下向きのRay2つを取得
        var rayTupleData = CreateRay_Down();

        //右か左どちらかでコライダーにぶつかったらtrue判定
        return
                Physics2D.Raycast(
                rayTupleData.Item1.origin,
                rayTupleData.Item1.direction,
                this.transform.localScale.y * 0.5f,
                groundMask
                ) ||
                Physics2D.Raycast(
                rayTupleData.Item2.origin,
                rayTupleData.Item2.direction,
                this.transform.localScale.y * 0.5f,
                groundMask
                );
    }

    /// <summary>
    /// 地面に埋まっているかを返す
    /// </summary>
    /// <returns>地面に接地している</returns>
    private bool CheckBury()
    {
        //下向きのRay2つを取得
        var rayTupleData = CreateRay_Down();

        //CheckGroundより短いRayを飛ばして判断する
        return
                Physics2D.Raycast(
                rayTupleData.Item1.origin,
                rayTupleData.Item1.direction,
                this.transform.localScale.y * 0.5f - 0.01f,
                groundMask
                ) ||
                Physics2D.Raycast(
                rayTupleData.Item2.origin,
                rayTupleData.Item2.direction,
                this.transform.localScale.y * 0.5f - 0.01f,
                groundMask
                );
    }

    /// <summary>
    /// 下向きのRayを生成する関数
    /// </summary>
    /// <returns>
    /// Item1 = 向かって右端が原点のRay
    /// Item2 = 向かって左端が原点のRay
    /// </returns>
    private (Ray, Ray) CreateRay_Down()
    {
        //右側のRayの原点を取得
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.x += this.transform.localScale.x * 0.4f;

        //左側のRayの原点を取得
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.x -= this.transform.localScale.x * 0.4f;

        //作成したRay2つを返す
        return (new Ray(rightOrigin, -this.transform.up),
                new Ray(leftOrigin, -this.transform.up));
    }

    /// <summary>
    /// Rayを生成する関数
    /// </summary>
    /// <returns>
    /// Item1 = 向かって右端が原点のRay
    /// Item2 = 向かって左端が原点のRay
    /// </returns>
    private (Ray, Ray) CreateRay_Up()
    {
        //右側のRayの原点を取得
        Vector3 rightOrigin = this.transform.position;
        rightOrigin.x += this.transform.localScale.x * 0.4f;

        //左側のRayの原点を取得
        Vector3 leftOrigin = this.transform.position;
        leftOrigin.x -= this.transform.localScale.x * 0.4f;

        //作成したRay2つを返す
        return (new Ray(rightOrigin, this.transform.up),
                new Ray(leftOrigin, this.transform.up));
    }

    /// <summary>
    /// 天井に達したかを返す関数
    /// </summary>
    private bool OnCeiling()
    {
        //上向きのRayを作成
        var rayTupleData = CreateRay_Up();

        //天井に左右いずれかのRayがぶつかったかを返す
        return
             Physics2D.Raycast(
                    rayTupleData.Item1.origin,
                    rayTupleData.Item1.direction,
                    this.transform.localScale.y * 0.5f,
                    groundMask) ||
           Physics2D.Raycast(
                    rayTupleData.Item2.origin,
                    rayTupleData.Item2.direction,
                    this.transform.localScale.y * 0.5f,
                    groundMask);


    }

    /// <summary>
    /// ジャンプ可能かを返す関数
    /// </summary>
    private bool JumpAble()
    {
        return _jumpCount > 0;
    }
}