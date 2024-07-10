using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatsuC : MonoBehaviour
{
    [SerializeField]
    [Tooltip("プレイヤー情報")]
    private GameObject _gameobjectPlayer;

    /// <summary>
    /// ダツ通常行動モード
    /// -3=死
    /// -2=ひるみ
    /// -1=気絶
    /// 0=通常時
    /// 1=攻撃準備
    /// 2=突進中
    /// 3=壁突き刺さり
    /// 4=初期位置に戻る
    /// </summary>
    private int _datsuMode;

    [SerializeField]
    [Tooltip("周回モード時の周回速度倍率")]

    private float _moveSpeed;

    [SerializeField]
    [Tooltip("周回モード時の直径倍率")]
    private float _moveMaxX;

    /// <summary>
    /// 周回モード時のSin代入変数
    /// </summary>
    private float _shukaiX;

    [SerializeField]
    [Tooltip("索敵範囲大きさ（合計pixel）")]
    private float _eyeSight;

    [SerializeField]
    [Tooltip("チャージ助走速度")]
    private float _chargeSpeed = 1.0f;

    [SerializeField, Tooltip("チャージ時間")]
    private float _chargeTime = 3.0f;

    /// <summary>
    /// 速度増分値
    /// </summary>
    private float _speedDelta;

    /// <summary>
    /// 突進時の方向指定
    /// </summary>
    private float _attackAngle;


    [SerializeField]
    [Tooltip("突進速度倍率")]
    private float _attackSpeed;

    /// <summary>
    /// 壁から出てから初めのポジションに戻る際の速度;
    /// </summary>
    private Vector3 _backSpeedDelta;

    /// <summary>
    /// 自分の座標
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// 初期位置
    /// </summary>
    private Vector3 _positionOwnFirst;

    /// <summary>
    /// 突進時Playerの座標
    /// </summary>
    private Vector3 _positionPlayerTarget;

    /// <summary>
    /// ダツレイキャスト定義
    /// </summary>
    private Ray _datsuRay;

    /// <summary>
    /// ダツto床レイキャスト衝突定義
    /// </summary>
    private RaycastHit2D _datsuToFloorRayHit;

    /// <summary>
    /// ダツtoPlayreレイキャスト衝突定義
    /// </summary>
    private RaycastHit2D _datsuToPlayerRayHit;

    /// <summary>
    /// 
    /// </summary>
    private Vector3 _lotationPlayerFirst;

    [SerializeField]
    [Tooltip("ダツスプライト変更元")]
    private SpriteRenderer _spriteDatsu;

    /// <summary>
    /// 気絶中
    /// </summary>
    private bool _isFainting;

    [SerializeField]
    [Tooltip("気絶時落下速度倍率")]
    private float _faintingDropSpeed = 1;

    /// <summary>
    /// GraphicC登録
    /// </summary>
    private GraphicC _graphicScrpt;

    /// <summary>
    /// ECoreC登録
    /// </summary>
    private EnemyCoreC _enemyCoreScript;

    // Start is called before the first frame update
    void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");

        //常にダメージを喰らう
        GetComponent<EnemyCoreC>().IsGetDamage = true;

        _datsuMode = 0;
        //初期位置登録
        _positionOwnFirst = transform.localPosition;
        Debug.DrawRay(_positionOwnNow, new Vector3(0, 0, 1), Color.red, 10.0f, false);

        _lotationPlayerFirst=transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

        //現在位置更新
        _positionOwnNow = transform.localPosition;

        //周回。Sinに与える変数を増加していく。
        if(_moveMaxX!=0) _shukaiX += 1 / _moveMaxX;



        //360度->0度
        if (_shukaiX >= 360) _shukaiX = 0;

        //通常時
        if (_datsuMode == 0)
        {
            //プレイヤー位置特定
            _positionPlayerTarget = _gameobjectPlayer.transform.position;
            
            //動かす。Sinに変数を入れて行ったり来たりを表現する
            transform.position += new Vector3((Mathf.Sin(_shukaiX) * Mathf.Deg2Rad) * _moveSpeed, 0);
            //折り返しでX反転する
            if (0 <= Mathf.Sin(_shukaiX) * Mathf.Deg2Rad) _spriteDatsu.flipX = true;
            else _spriteDatsu.flipX = false;

            //発見
            if (_positionOwnNow.x - _eyeSight / 2 <= _positionPlayerTarget.x
                && _gameobjectPlayer.transform.position.x <= _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("発見");
                //GetAngleを用いて向きを割り出す
                _attackAngle = GetAngle(_gameobjectPlayer.transform.position - _positionOwnNow);
                transform.localEulerAngles = new Vector3(0, 0, -_attackAngle+90);
                //攻撃チャージ開始
                StartCoroutine("AttackCharge");
                //モード変更
                _datsuMode = 1;
            }
        }

        //敵発見
        else if (_datsuMode == 1)
        {
            _attackAngle = GetAngle(_gameobjectPlayer.transform.position - _positionOwnNow);
            //軸合わせしてくるシステム
            transform.localEulerAngles = new Vector3(0, 0, -_attackAngle + 90);
            transform.position -= GetDirection(_attackAngle) * _chargeSpeed;
            
        }

        //突撃
        else if (_datsuMode == 2)
        {
            transform.position += GetDirection(_attackAngle) * _attackSpeed;
            //光線登録
            _datsuRay = new Ray(_positionOwnNow, new Vector3(0,0,0));
            //床レイヤーにRayを飛ばす
            _datsuToFloorRayHit = Physics2D.Raycast(_datsuRay.origin+transform.right*2.0f, _datsuRay.direction, 10, 8);
            if (_datsuToFloorRayHit.collider)
            {
                //刺さり行動開始
                StartCoroutine("Stinging");
                _datsuMode = 3;
            }
            //PlayerレイヤーにRayを飛ばす
            _datsuToPlayerRayHit = Physics2D.Raycast(_datsuRay.origin + transform.right * 2.0f, _datsuRay.direction, 10, 128);
            if (_datsuToPlayerRayHit.collider)
            {
                Debug.Log("ダツこうげき！");
                Debug.Log(_datsuToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().name);
                Debug.Log(_datsuToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().Hp);
                _datsuToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().PlayerDamage(1);
                _datsuMode = 4;
                StartCoroutine("BackToFirstPosition");
            }
        }

        //壁突き刺さり中
        else if (_datsuMode == 3)
        {
            transform.position -= GetDirection(_attackAngle) * _speedDelta;
        }

        //初期位置に戻る
        else if (_datsuMode == 4)
        {
            transform.position += _backSpeedDelta;
        }

        if (_datsuMode == -1)
        {
            //床にあたるまで落下する
            _datsuRay = new Ray(_positionOwnNow, -transform.forward);
            //床レイヤーにRayを飛ばす
            _datsuToFloorRayHit = Physics2D.Raycast(_datsuRay.origin, _datsuRay.direction, 10,8);
            if (!_datsuToFloorRayHit.collider)
            {
                transform.position += -transform.up * _faintingDropSpeed;
            }
        }

        //死行動
        else if (_datsuMode == -3)
        {
            transform.localEulerAngles = Vector3.zero;
            _spriteDatsu.flipY = true;
            //_deathMoveSpeed -= _deathMoveDeltaY;
            //ひっくり返って落ちてゆく
            transform.position -= new Vector3(0, 0.07f, 0);
        }

        //グラフィックがさかさまにならないように調節。回転が90~270になったらY反転
        if (_datsuMode >= 0)
        {
            if (90 <= transform.localEulerAngles.z && transform.localEulerAngles.z < 270) _spriteDatsu.flipY = true;
            else _spriteDatsu.flipY = false;
        }
    }


    /// <summary>
    /// 範囲内にplayerいるかどうかチェック
    /// </summary>
    /// <param name="positionOwn">自分の座標</param>
    /// <param name="eyeSight">Playerの座標</param>
    /// <param name="positionPlayer"></param>
    /// <returns>いますか？TorF</returns>
    private bool InSphere(Vector3 positionOwn, float eyeSight, Vector3 positionPlayer)
    {
        var sum = 0f;
        for (var i = 0; i < 3; i++)
            sum += Mathf.Pow(positionOwn[i] - positionPlayer[i], 2);
        return sum <= Mathf.Pow(eyeSight, 2f);
    }

    /// <summary>
    /// 3秒間チャージ(0.03*100)
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCharge()
    {
        GetComponent<GraphicC>().ResetAnimation(5);
        _spriteDatsu.flipX = true;
        yield return new WaitForSeconds(_chargeTime);
        GetComponent<GraphicC>().ResetAnimation(0);
        _datsuMode = 2;
    }

    /// <summary>
    /// 死アクションスタート
    /// </summary>
    public void OnDeath()
    {
        GetComponent<GraphicC>().ResetAnimation(1);
        StopAllCoroutines();
        _datsuMode = -3;
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// 引っこ抜き行動
    /// </summary>
    /// <returns></returns>
    private IEnumerator Stinging()
    {
        //突き刺さる
        /*
        for (int i = 0; i < 10; i++)
        {
            transform.position += GetDirection(_attackAngle) * _attackSpeed;
            yield return new WaitForSeconds(0.03f);
        }
        */
        _speedDelta = 0;
        yield return new WaitForSeconds(0.5f);
        //突き刺さった分戻る
        _speedDelta = _attackSpeed;
        yield return new WaitForSeconds(0.09f);
        _speedDelta = 0;
        yield return new WaitForSeconds(0.5f);
        //向きを正面に戻す
        transform.localEulerAngles = new Vector3(0, 0, 0);
        //壁からスタート位置に戻る行動開始
        StartCoroutine("BackToFirstPosition");
        _datsuMode = 4;
    }

    /// <summary>
    /// 壁からスタート位置に戻る
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackToFirstPosition()
    {
        GetComponent<GraphicC>().ResetAnimation(0);
        //初期位置と現在地から戻る方向とスピードを割り出す
        _backSpeedDelta = (_positionOwnFirst - _positionOwnNow) / 50;
        transform.localEulerAngles += new Vector3(0, 0, 180);
        //1.5秒かけて戻る
        yield return new WaitForSeconds(1.5f);
        _shukaiX = 0;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        //通常時に戻る
        _datsuMode = 0;
    }

    /// <summary>
    /// 気絶発生
    /// </summary>
    public void BeFainting()
    {
        
        if (_datsuMode!=-1)
        //_isFainting = true;
        StartCoroutine("DoFainting");
    }

    /// <summary>
    /// 気絶アクション
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoFainting()
    {
        GetComponent<GraphicC>().ResetAnimation(11);
        Debug.Log("気絶");
        //すべてのコルーチンを止める
        StopCoroutine("AttackCharge");
        StopCoroutine("Stinging");
        StopCoroutine("BackToFirstPosition");
        _datsuMode = -1;
        transform.localEulerAngles = _lotationPlayerFirst;
        _spriteDatsu.flipY = true;

        //床まで落ちる
        yield return new WaitForSeconds(10.0f);
        //壁からスタート位置に戻る行動開始
        StartCoroutine("BackToFirstPosition");
        _datsuMode = 4;
        Debug.Log("気絶復帰");
    }


    /// <summary>
    /// 被ダメージ
    /// </summary>
    public void GetDamage()
    {

    }

    /// <summary>
    /// ownからtargetへ進む際の加算値をVector3で割り出す
    /// </summary>
    /// <param name="own">進ませたいオブジェクトの座標</param>
    /// <param name="target">目的座標</param>
    /// <returns>加算値。これをownに足すとtargetに向かって進む</returns>
    private Vector3 GetMoveTarget(Vector3 own, Vector3 target)
    {
        Vector3 direction = target - own;
        float angleForTarget = GetAngle(direction);
        return GetDirection(angleForTarget);
    }

    /// <summary>
    /// Aから見たBの向きを特定
    /// </summary>
    /// <param name="direction">Vector3 B-A</param>
    /// <returns>Aから見たBの向き</returns>
    private float GetAngle(Vector3 direction)
    {
        float rad = Mathf.Atan2(direction.x, direction.y);
        return rad * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 進みたい方向を元にVector3の加算値を割り出す
    /// </summary>
    /// <param name="angle">進みたい方向</param>
    /// <returns>加算値。これを足すとangleに向かって進む</returns>
    private Vector3 GetDirection(float angle)
    {
        Vector3 direction = new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
        return direction;
    }

}
