using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    /// <summary>
    /// ボス行動モード
    /// 0=通常時
    /// 1=発見
    /// 2=攻撃
    /// 3=ふっとび中
    /// 4=床突き刺さり
    /// 5=復帰中
    /// 6=逃走（撃破）
    /// </summary>
    private int _boss1Mode;

    /// <summary>
    /// ボス攻撃モード
    /// 0=なし
    /// 1=ツメ
    /// </summary>
    private int _boss1Attack;

    /// <summary>
    /// 行動カウントアップ
    /// </summary>
    private int _bossCount = 0;

    [SerializeField]
    [Tooltip("プレイヤー情報")]
    private GameObject _gameobjectPlayer;

    /// <summary>
    /// 自分の座標
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// 自分の初期位置
    /// </summary>
    private Vector3 _positionOwnFirst;

    /// <summary>
    /// Playerの座標
    /// </summary>
    private Vector3 _positionPlayerNow;

    [SerializeField, Tooltip("衝撃波食らった時の1フレームの回転（540の約数で入力）")]
    private int _smashRotationValue = 30;

    /// <summary>
    /// 衝撃波食らった時のジャンプ
    /// </summary>
    private float _smashJumpValue;

    /// <summary>
    /// 衝撃波食らった時の割り率
    /// </summary>
    private int _smashCount = 0;

    [SerializeField,Tooltip("Rayディレイ")]
    private float _rayPositionDaray;

    [SerializeField,Tooltip("未発見歩行スピード(cm/f)")]
    private float _walkSpeed;

    [SerializeField,Tooltip("Uターンする移動距離(0を入れると無限に進む)")]
    private float _turnBackDistance;

    [SerializeField,Tooltip("索敵範囲大きさ（合計pixel）")]
    private float _eyeSight;

    [SerializeField,Tooltip("追尾歩行スピード(cm/f)")]
    private float _walkSpeedChase;

    [SerializeField,Tooltip("ブレスプレハブ")]
    private BressC _bressPf;

    [SerializeField, Tooltip("EXPプレハブ")]
    private ExpC _bubblePf;

    /// <summary>
    /// ヤドカリレイキャスト定義
    /// </summary>
    private Ray _boss1Ray;

    /// <summary>
    /// ヤドカリtoプレイヤー衝突判定
    /// </summary>
    private RaycastHit2D _boss1ToPlayerRayHit;

    /// <summary>
    /// ヤドカリto壁衝突判定
    /// </summary>
    private RaycastHit2D _boss1ToWallRayHitR, _boss1ToWallRayHitL;

    /// <summary>
    /// ヤドカリto床衝突判定
    /// </summary>
    private RaycastHit2D _boss1ToFloorRayHit;

    /// <summary>
    /// 衝撃波ひっくりかえりカウント
    /// </summary>
    private int _sonicHp=3;

    /// <summary>
    /// GraphicC登録
    /// </summary>
    private GraphicC _graphicScrpt;

    /// <summary>
    /// ECoreC登録
    /// </summary>
    private EnemyCoreC _enemyCoreScript;

    [SerializeField]
    [Tooltip("ヤドカリスプライト変更元")]
    private SpriteRenderer _spriteYadokari;

    /// <summary>
    /// 実行コルーチン
    /// </summary>
    private Coroutine _currentCoroutine;

    /// <summary>
    /// 重力
    /// </summary>
    private float _gravityScale = 0;

    [SerializeField, Tooltip("重力最大値、重力加速度")]
    private float _gravityMax = 0.02f, _gravityDelta = 0.01f;


    // Start is called before the first frame update
    void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");

        //GetComponent<GraphicC>().ResetAnimation(0);
        //無敵モードオン
        GetComponent<EnemyCoreC>()._isGetBlowAble = false;
        //初期位置登録
        _positionOwnFirst = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //現在位置更新
        _positionOwnNow = transform.localPosition;
        //プレイヤー位置特定
        _positionPlayerNow = _gameobjectPlayer.transform.position;
    }

    void FixedUpdate()
    {
        _boss1Ray = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));

        //壁あたり判定（当たったら引き返し）
        _boss1ToWallRayHitR = Physics2D.Raycast(_boss1Ray.origin + new Vector3(-_rayPositionDaray, -4.0f, 0), _boss1Ray.direction, 10, 8);
        _boss1ToWallRayHitL = Physics2D.Raycast(_boss1Ray.origin + new Vector3(_rayPositionDaray, -4.0f, 0), _boss1Ray.direction, 10, 8);

        //床あたり判定（当たらなくなったら引き返し）
        _boss1ToFloorRayHit = Physics2D.Raycast(_boss1Ray.origin - transform.up * _rayPositionDaray, _boss1Ray.direction, 10, 8);
        //床まで落ちる
        if (_boss1ToFloorRayHit)
        {
            _gravityScale = 0;
        }
        else
        {
            transform.position -= transform.up * _gravityScale;
            //重力
            if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
            else _gravityScale = _gravityMax;
            Debug.Log("崖");
            TurnBack();
        }

        //通常時
        if (_boss1Mode == 0)
        {
            //移動
            transform.position += transform.right * _walkSpeed;

            //座標Uターン
            if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
            {
                TurnBack();
            }

            //レイ定義
            if (_boss1ToWallRayHitR || _boss1ToWallRayHitL)
            {
                TurnBack();
            }
            

            //発見
            if (_positionOwnNow.x - _eyeSight / 2 <= _positionPlayerNow.x
                && _gameobjectPlayer.transform.position.x <= _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("発見");
                _boss1Mode = 1;
            }
        }
        if (_boss1Mode == 1)
        {

            if (_boss1Attack == 0)
            {
                _bossCount++;
                if (_bossCount >= 110)
                {
                    _boss1Attack = Random.Range(1, 3);
                    Debug.Log("ボスコード：" + _boss1Attack);
                    _bossCount = 0;
                }
            }

            //ツメ
            if (_boss1Attack == 1)
            {
                GetComponent<GraphicC>().ResetAnimation(10);
                //プレイヤー見続ける
                if (_positionOwnNow.x-_rayPositionDaray <= _positionPlayerNow.x)
                {

                    //_spriteYadokari.flipX = true;
                    transform.position += transform.right * _walkSpeedChase;
                }
                else
                {
                    //_spriteYadokari.flipX = false;
                    transform.position -= transform.right * _walkSpeedChase;
                }

                //座標Uターン
                if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                    || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
                {
                    TurnBack();
                }

                //レイ定義
                if (_boss1ToWallRayHitR || _boss1ToWallRayHitL)
                {
                    TurnBack();
                }



                if (_boss1Attack == 1)
                {
                    //プレイヤーあたり判定（当たったら攻撃）
                    _boss1ToPlayerRayHit = Physics2D.Raycast(_boss1Ray.origin- new Vector3(_rayPositionDaray, 4.0f, 0), _boss1Ray.direction , 10, 128);
                    if (_boss1ToPlayerRayHit.collider)
                    {
                        _boss1Mode = 2;
                        _currentCoroutine = StartCoroutine(AttackAnimation(_boss1ToPlayerRayHit.collider.gameObject));
                    }
                }

                _bossCount++;
                if (_bossCount >= 100)
                {
                    GetComponent<GraphicC>().ResetAnimation(0);
                    _bossCount = 0;
                    _boss1Attack = 0;
                }
            }

            //泡
            if (_boss1Attack == 2)
            {
                if (_bossCount < 50)
                {
                    Instantiate(_bubblePf, _positionOwnNow + new Vector3(-2.0f, -4.0f, 0), transform.localRotation).EShot1(Random.Range(0, 360), 0.1f, 0.3f);
                }
                if (_bossCount == 50)
                {
                    Instantiate(_bressPf, _positionOwnNow + new Vector3(-2.0f, -4.0f, 0), transform.localRotation).EShot1(180, 0.01f, 0.01f);
                }

                _bossCount++;
                if (_bossCount >= 100)
                {
                    _bossCount = 0;
                    _boss1Attack = 0;
                }
            }


            //見失い
            /*
            if (_positionOwnNow.x - _eyeSight / 2 > _positionPlayerNow.x
                || _gameobjectPlayer.transform.position.x > _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("見失い");
                //GetComponent<GraphicC>().ResetAnimation(0);
                LookSet();
                _boss1Mode = 0;
            }
            */
        }


        //ふっとび大回転
        if (_boss1Mode == 3)
        {
            transform.localEulerAngles += transform.forward * -_smashRotationValue;
            transform.localPosition += new Vector3(0, _smashJumpValue, 0);
            _smashJumpValue -= 0.06f;
            _smashCount++;
            //一回転半したら突き刺さる
            if (_smashCount >= 540 / _smashRotationValue)
            {
                Debug.Log("ぐさり！！！");
                transform.localPosition += new Vector3(0, _smashJumpValue, 0);
                GetComponent<EnemyCoreC>()._isGetBlowAble = true;
                _smashCount = 0;
                _smashJumpValue = 0;


                _boss1Mode = 4;
                StopAllCoroutines();
                _currentCoroutine = null;
                _currentCoroutine = StartCoroutine(DownAction());
                _smashJumpValue = (540 / _smashRotationValue) * 0.03f;
            }
        }

        if (_boss1Mode == 5)
        {
            transform.localEulerAngles += transform.forward * _smashRotationValue;
            transform.localPosition += new Vector3(0, _smashJumpValue, 0);
            _smashJumpValue -= 0.06f;
            _smashCount++;
            if (_smashCount >= 540 / _smashRotationValue)
            {
                Debug.Log("もどり！！！");
                transform.localPosition += new Vector3(0, _smashJumpValue, 0);
                GetComponent<EnemyCoreC>()._isGetBlowAble = false;
                _smashCount = 0;
                _smashJumpValue = 0;
                GetComponent<GraphicC>().ResetAnimation(0);
                _sonicHp = 3;
                _boss1Mode = 0;
            }
        }

        //死行動
        else if (_boss1Mode == 6)
        {
            _spriteYadokari.flipY = true;
            //_deathMoveSpeed -= _deathMoveDeltaY;
            //ひっくり返って落ちてゆく
            transform.position += new Vector3(0, 0.01f, 0);
        }


    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator AttackAnimation(GameObject targetPlayer)
    {
        GetComponent<GraphicC>().ResetAnimation(2);
        Debug.Log("ヤドカリこうげき！");
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().name);
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().Hp);
        targetPlayer.GetComponentInParent<PlayerManager>().PlayerDamage(1);
        yield return new WaitForSeconds(0.5f);
        GetComponent<GraphicC>().ResetAnimation(0);
        _boss1Mode = 1;
        _bossCount = 0;
        _boss1Attack = 0;

    }

    /// <summary>
    /// 衝撃波食らってひっくりかえる
    /// </summary>
    public void Smashed()
    {
        if(_boss1Attack == 1)
        {

            _sonicHp--;
        }
        if (_sonicHp <= 0)
        {
            //通常か移動中なら衝撃波効果あり
            if (0 <= _boss1Mode && _boss1Mode <= 1)
            {
                _bossCount = 0;
                _boss1Attack = 0;
                GetComponent<GraphicC>().ResetAnimation(11);
                _boss1Mode = 3;
                _smashJumpValue = (540 / _smashRotationValue) * 0.03f;
            }
        }

    }

    /// <summary>
    /// 床突き刺さりアクション
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownAction()
    {
        yield return new WaitForSeconds(15.0f);
        _boss1Mode = 5;
    }

    /// <summary>
    /// 被ダメージ
    /// </summary>
    public void GetDamage()
    {
        if (_boss1Mode == 4)
        {
            Debug.Log("殻が！");
            //StartCoroutine(KnockBack());
        }
        else
        {
            Debug.Log("殻に守られた俺にパンチは効かないねぇ");
        }
    }

    /// <summary>
    /// 死アクションスタート
    /// </summary>
    public void OnDeath()
    {
        GetComponent<GraphicC>().ResetAnimation(1);
        StopAllCoroutines();
        _currentCoroutine = null;
        _boss1Mode = 6;
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// 壁崖方向転換
    /// </summary>
    private void TurnBack()
    {
        transform.position += -transform.right * _walkSpeed * 2;
        _walkSpeed *= -1;
        //LookSet();
    }

    /// <summary>
    /// 左右向き合わせ
    /// </summary>
    private void LookSet()
    {
        if (_walkSpeed < 0) _spriteYadokari.flipX = false;
        else _spriteYadokari.flipX = true;
    }
}
