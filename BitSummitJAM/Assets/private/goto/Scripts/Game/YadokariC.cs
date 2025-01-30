using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YadokariC : MonoBehaviour
{

    /// <summary>
    /// ヤドカリ行動モード
    /// 0=通常時
    /// 1=発見
    /// 2=攻撃
    /// 3=ふっとび中
    /// 4=床突き刺さり
    /// 5=復帰中
    /// 6=逃走（撃破）
    /// 7=圧死
    /// </summary>
    /// [SerializeField]
    private int _yadokariMode=0;

    /// <summary>
    /// スタート後着地したか
    /// </summary>
    private bool _isSet;

    /// <summary>
    /// プレイヤー情報
    /// </summary>
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

    [SerializeField,Tooltip("衝撃波食らった時の1フレームの回転（540の約数で入力）")]
    private int _smashRotationValue=30;

    /// <summary>
    /// 衝撃波食らった時のジャンプ
    /// </summary>
    private float _smashJumpValue;

    /// <summary>
    /// 衝撃波食らった時の割り率
    /// </summary>
    private int _smashCount = 0;

    [SerializeField]
    [Tooltip("未発見歩行スピード(cm/f)")]
    private float _walkSpeed;

    [SerializeField]
    [Tooltip("Uターンする移動距離(0を入れると無限に進む)")]
    private float _turnBackDistance;

    [SerializeField]
    [Tooltip("索敵範囲大きさ（合計pixel）")]
    private float _eyeSight;

    [SerializeField]
    [Tooltip("追尾歩行スピード(cm/f)")]
    private float _walkSpeedChase;

    /// <summary>
    /// ヤドカリレイキャスト定義
    /// </summary>
    private Ray _yadokariRay;

    /// <summary>
    /// ヤドカリtoプレイヤー衝突判定
    /// </summary>
    private RaycastHit2D _yadokariToPlayerRayHit;

    /// <summary>
    /// ヤドカリto壁衝突判定
    /// </summary>
    private RaycastHit2D _yadokariToWallRayHitR, _yadokariToWallRayHitL;

    /// <summary>
    /// ヤドカリto床衝突判定
    /// </summary>
    private RaycastHit2D _yadokariToFloorRayHit;

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
    private float _gravityMax=0.02f, _gravityDelta=0.01f;

    [SerializeField]
    private AudioClip _acDeath, _acDamage, _acBlocked,_acClow, _acSmashed,_acWakeUp,_acStinging;


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
        _yadokariRay = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));

        //壁あたり判定（当たったら引き返し）
        _yadokariToWallRayHitR = Physics2D.Raycast(_yadokariRay.origin + (transform.right * 0.5f) + (-transform.up * transform.localScale.y * 0.3f), _yadokariRay.direction, 10, 8);
        _yadokariToWallRayHitL = Physics2D.Raycast(_yadokariRay.origin - (transform.right * 0.5f) + (-transform.up * transform.localScale.y * 0.3f), _yadokariRay.direction, 10, 8);
        //床あたり判定（当たらなくなったら引き返し）
        _yadokariToFloorRayHit = Physics2D.Raycast(_yadokariRay.origin - (transform.up * transform.localScale.y * 1.0f), _yadokariRay.direction, 10, 8);

        //壁に挟まれたら埋まっているとみなし這い上がる(死んでないとき限定)
        if (_yadokariToWallRayHitR && _yadokariToWallRayHitL && _yadokariToFloorRayHit&&_yadokariMode<6)
        {
            transform.position += transform.up * 0.2f;
        }

        //通常時
        if (_yadokariMode == 0)
        {
            //移動
            transform.position += transform.right * _walkSpeed;

            //座標Uターン
            /*if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
            {
                TurnBack();
            }*/

            if (_yadokariToWallRayHitR || _yadokariToWallRayHitL)
            {
                TurnBack();
            }

            //床まで落ちる
            if (_yadokariToFloorRayHit)
            {
                _isSet = true;
                _gravityScale = 0;
            }
            else
            {
                transform.position -= transform.up * _gravityScale;
                //重力
                if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
                else _gravityScale = _gravityMax;
                Debug.Log("崖");
                //スタート後着地するまで回転しない。プルプルしちゃう
                if (_isSet)
                {
                    TurnBack();
                }
            }

            //発見
            if (_positionOwnNow.x - _eyeSight / 2 <= _positionPlayerNow.x
                && _gameobjectPlayer.transform.position.x <= _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("発見");
                GetComponent<GraphicC>().ResetAnimation(10);
                _yadokariMode = 1;
            }
        }

        //追尾
        if (_yadokariMode == 1)
        {
            //プレイヤー見続ける
            if (_positionOwnNow.x <= _positionPlayerNow.x)
            {

                _spriteYadokari.flipX = true;
                //壁も崖もなければ進む
                if (!_yadokariToWallRayHitR && !_yadokariToWallRayHitL&& _yadokariToFloorRayHit)
                {
                    
                    transform.position += transform.right * _walkSpeedChase;
                }
                else
                {
                    transform.position -= transform.right * _walkSpeedChase*10;
                }
                
            }
            else
            {
                _spriteYadokari.flipX = false;
                //壁も崖もなければ進む
                if (!_yadokariToWallRayHitR && !_yadokariToWallRayHitL&& _yadokariToFloorRayHit)
                {
                    transform.position -= transform.right * _walkSpeedChase;
                }
                else
                {
                    transform.position += transform.right * _walkSpeedChase * 10;
                }
            }

            //床まで落ちる
            if (_yadokariToFloorRayHit)
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
            }

            //プレイヤーあたり判定（当たったら攻撃）
            _yadokariToPlayerRayHit = Physics2D.Raycast(_yadokariRay.origin, _yadokariRay.direction, 10, 128);
            if (_yadokariToPlayerRayHit.collider)
            {
                _yadokariMode = 2;
                _currentCoroutine= StartCoroutine(AttackAnimation(_yadokariToPlayerRayHit.collider.gameObject));
            }

            //見失い
            if (_positionOwnNow.x - _eyeSight / 2 > _positionPlayerNow.x
                || _gameobjectPlayer.transform.position.x > _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("見失い");
                GetComponent<GraphicC>().ResetAnimation(0);
                LookSet();
                _yadokariMode = 0;
            }
        }
        

        //ふっとび大回転
        if (_yadokariMode == 3)
        {
            transform.localEulerAngles += transform.forward * -_smashRotationValue;
            transform.localPosition += new Vector3(0, _smashJumpValue, 0);
            _smashJumpValue-=0.06f;
            _smashCount++;
            //一回転半したら突き刺さる
            if (_smashCount>=540/_smashRotationValue)
            {
                Debug.Log("ぐさり！！！");
                transform.localPosition += new Vector3(0, _smashJumpValue, 0);
                GetComponent<EnemyCoreC>()._isGetBlowAble = true;
                _smashCount = 0;
                _smashJumpValue = 0;


                _yadokariMode = 4;
                StopAllCoroutines();
                _currentCoroutine = null;
                _currentCoroutine = StartCoroutine(DownAction());
                _smashJumpValue = (540 / _smashRotationValue)*0.03f;
            }
        }

        if (_yadokariMode == 5)
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

                _yadokariMode = 0;
            }
        }

        //死行動
        else if (_yadokariMode==6)
        {
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            transform.localEulerAngles = Vector3.zero;
            _spriteYadokari.flipY = false;
            _spriteYadokari.flipX = true;
            //走っていく
            transform.position += new Vector3(0.01f, -0.01f, 0);
        }

        //圧死行動
        else if (_yadokariMode == 7)
        {
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            transform.localEulerAngles = Vector3.zero;
            _spriteYadokari.flipY = false;
            _spriteYadokari.flipX = true;
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
        GetComponent<AudioSource>().PlayOneShot(_acClow);
        Debug.Log("ヤドカリこうげき！");
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().name);
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().Hp);
        targetPlayer.GetComponentInParent<PlayerManager>().PlayerDamage(1);
        yield return new WaitForSeconds(0.5f);
        GetComponent<GraphicC>().ResetAnimation(10);
        _yadokariMode = 1;
    }

    /// <summary>
    /// 衝撃波食らってひっくりかえる
    /// </summary>
    public void Smashed()
    {
        //通常か移動中なら衝撃波効果あり
        if (0 <= _yadokariMode && _yadokariMode <= 1)
        {
            GetComponent<AudioSource>().PlayOneShot(_acSmashed);
            GetComponent<GraphicC>().ResetAnimation(11);
            _yadokariMode = 3;
            _smashJumpValue = (540 / _smashRotationValue) * 0.03f;
        }
    }

    /// <summary>
    /// 床突き刺さりアクション
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownAction()
    {
        yield return new WaitForSeconds(3.0f);
        _yadokariMode = 5;
        GetComponent<AudioSource>().PlayOneShot(_acWakeUp);
    }
    
    /// <summary>
    /// 被ダメージ
    /// </summary>
    public void GetDamage()
    {
        if (_yadokariMode == 4)
        {
            GetComponent<AudioSource>().PlayOneShot(_acDamage);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(_acBlocked);
        }
    }

    /// <summary>
    /// 死アクションスタート
    /// </summary>
    public void OnDeath()
    {
        gameObject.layer = 0;
        GetComponent<AudioSource>().PlayOneShot(_acDeath);
        GetComponent<GraphicC>().ResetAnimation(1);
        StopAllCoroutines();
        _currentCoroutine = null;
        _yadokariMode = 6;
        Destroy(gameObject, 5.0f);
    }

    /// <summary>
    /// 圧死
    /// </summary>
    public void OnScrap()
    {
        gameObject.layer = 0;
        GetComponent<AudioSource>().PlayOneShot(_acDeath);
        GetComponent<GraphicC>().ResetAnimation(16);
        StopAllCoroutines();
        _currentCoroutine = null;
        _yadokariMode = 7;
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// 壁崖方向転換
    /// </summary>
    private void TurnBack()
    {
        transform.position -= transform.right * _walkSpeed * 5;
        _walkSpeed *= -1;
        LookSet();
    }

    /// <summary>
    /// 左右向き合わせ
    /// </summary>
    private void LookSet()
    {
        if (_walkSpeed < 0) _spriteYadokari.flipX = false;
        else _spriteYadokari.flipX = true;
    }

    private void OnBecameInvisible()
    {
        //通常時に画面の外にいたら休止
        if (_yadokariMode == 0) _yadokariMode = 999;
    }

    private void OnBecameVisible()
    {
        //休止時に画面の中にいたら再開
        if (_yadokariMode == 999) _yadokariMode = 0;
    }

}
