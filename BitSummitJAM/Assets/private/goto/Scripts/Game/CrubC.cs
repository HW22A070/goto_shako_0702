using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrubC : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// カニ通常行動モード
    /// -3=死
    /// -2=被弾
    /// -1=衝撃波
    /// 0=通常
    /// 1=攻撃
    /// 999=画面外
    /// </summary>
    private int _crubMode;

    /// <summary>
    /// スタート後着地したか
    /// </summary>
    private bool _isSet;

    [SerializeField]
    [Tooltip("歩行スピード(cm/f)")]
    private float _walkSpeed;

    [SerializeField]
    [Tooltip("Uターンする移動距離(0を入れると無限に進む)")]
    private float _turnBackDistance;

    /// <summary>
    /// 自分の座標
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// 自分の初期位置
    /// </summary>
    private Vector3 _positionOwnFirst;

    /// <summary>
    /// カニレイキャスト定義
    /// </summary>
    private Ray _crubRay;

    /// <summary>
    /// カニtoプレイヤー衝突判定
    /// </summary>
    private RaycastHit2D _crubToPlayerRayHit;

    /// <summary>
    /// カニto壁衝突判定
    /// </summary>
    private RaycastHit2D _crubToWallRayHitR, _crubToWallRayHitL;

    /// <summary>
    /// カニto床衝突判定
    /// </summary>
    private RaycastHit2D _crubToFloorRayHit;

    [SerializeField]
    [Tooltip("カニスプライト変更元")]
    private SpriteRenderer _spriteCrub;

    /// <summary>
    /// GraphicC登録
    /// </summary>
    //private GraphicC _graphicScrpt;

    [SerializeField]
    [Tooltip("死ぬときの初期ジャンプ速度")]
    private float _deathMoveDeltaY = 0.03f;

    [SerializeField]
    [Tooltip("死ぬときの落下速度倍率")]
    private float _deathMoveSpeed = 1;

    /// <summary>
    /// 重力
    /// </summary>
    private float _gravityScale = 0;

    [SerializeField,Tooltip("重力最大値、重力加速度")]
    private float _gravityMax=0.02f, _gravityDelta=0.01f;


    [SerializeField]
    private AudioClip _acDeath, _acDamage, _acBlocked,_acGetSonic;


    // Start is called before the first frame update
    void Start()
    {
        //初期位置登録
        _positionOwnFirst = transform.localPosition;

        //常にダメージを喰らう
        GetComponent<EnemyCoreC>().IsGetDamage = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!MenuC.Pouse)
        {
            //現在位置更新
            _positionOwnNow = transform.localPosition;

            //通常時
            if (_crubMode == 0)
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
                _crubRay = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));

                //壁あたり判定（当たったら引き返し）
                _crubToWallRayHitR = Physics2D.Raycast(_crubRay.origin + (transform.right *0.5f) + (-transform.up * transform.localScale.y * 0.4f), _crubRay.direction, 10, 8);
                _crubToWallRayHitL = Physics2D.Raycast(_crubRay.origin + (-transform.right * 0.5f) + (-transform.up * transform.localScale.y * 0.4f), _crubRay.direction, 10, 8);
                //床あたり判定（当たらなくなったら引き返し）
                _crubToFloorRayHit = Physics2D.Raycast(_crubRay.origin - (transform.up * transform.localScale.y * 0.5f), _crubRay.direction, 10, 8);

                //生き埋め対策
                if (_crubToWallRayHitR && _crubToWallRayHitL&& _crubToFloorRayHit)
                {
                    transform.position += transform.up * 0.3f;
                }
                //壁ターン
                if (_crubToWallRayHitR || _crubToWallRayHitL)
                {
                    TurnBack();
                }
                //床ターン
                if (_crubToFloorRayHit)
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

                //プレイヤーあたり判定（当たったら攻撃）
                _crubToPlayerRayHit = Physics2D.Raycast(_crubRay.origin -= transform.up * 0.5f * transform.localScale.x, _crubRay.direction, 10, 128);
                if (_crubToPlayerRayHit)
                {
                    Debug.Log("敵か？");
                    _crubMode = 1;
                    StartCoroutine(AttackAnimation(_crubToPlayerRayHit.collider.gameObject));
                }
            }

            //ノックバック
            else if (_crubMode == -1)
            {
                //床あたり判定（当たらなくなったら落ちる）
                /*
                _crubRay = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));
                _crubToFloorRayHit = Physics2D.Raycast(_crubRay.origin - transform.up * 1.3f, _crubRay.direction, 10, 8);
                if (!_crubToFloorRayHit)
                {
                    transform.position += new Vector3(0, -0.01f, 0);
                }
                */
            }

            //死行動
            else if (_crubMode == -3)
            {
                _spriteCrub.flipY = true;
                _deathMoveSpeed -= _deathMoveDeltaY;
                //ひっくり返って落ちてゆく
                transform.position += new Vector3(0, _deathMoveSpeed, 0);
            }
        }

    }

    private void OnBecameInvisible()
    {
        //通常時に画面の外にいたら休止
        if (_crubMode == 0) _crubMode = 999;
    }

    private void OnBecameVisible()
    {
        //休止時に画面の中にいたら再開
        if (_crubMode == 999) _crubMode = 0;
    }


    /// <summary>
    /// Uターン
    /// </summary>
    private void TurnBack()
    {
        transform.position += -transform.right * _walkSpeed * 5;
        _walkSpeed *= -1;

        
        
        if (_walkSpeed < 0) _spriteCrub.flipX = false;
        else _spriteCrub.flipX = true;
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator AttackAnimation(GameObject targetPlayer)
    {
        //Debug.Log("カニこうげき！");
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().name);
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().Hp);
        targetPlayer.GetComponentInParent<PlayerManager>(). PlayerDamage(1);
        yield return new WaitForSeconds(0.5f);
        _crubMode = 0;
    }

    /// <summary>
    /// 被ダメージ
    /// </summary>
    public void GetDamage()
    {
        if (_crubMode >= -1)
        {
            StartCoroutine(KnockBack());
        }
    }

    /// <summary>
    /// 被衝撃波
    /// </summary>
    public void GetSonic()
    {
        if (_crubMode >= 0)
        {
            StartCoroutine(KnockBackSonic());
        }
    }

    /// <summary>
    /// 衝撃波
    /// </summary>
    /// <returns></returns>
    private IEnumerator KnockBackSonic()
    {
        GetComponent<AudioSource>().PlayOneShot(_acGetSonic);
        GetComponent<GraphicC>().ResetAnimation(3);
        //transform.position += transform.up * 1.0f;
        _crubMode = -1;
        yield return new WaitForSeconds(2.0f);
        _crubMode = 0;
        GetComponent<GraphicC>().ResetAnimation(0);

    }

    /// <summary>
    /// ノックバック
    /// </summary>
    /// <returns></returns>
    private IEnumerator KnockBack()
    {
        GetComponent<AudioSource>().PlayOneShot(_acDamage);
        GetComponent<GraphicC>().ResetAnimation(3);
        //transform.position += transform.up * 1.0f;
        _crubMode = -2;
        yield return new WaitForSeconds(1.0f);
        _crubMode = 0;
        GetComponent<GraphicC>().ResetAnimation(0);

    }

    /// <summary>
    /// 死アクションスタート
    /// </summary>
    public void OnDeath()
    {
        GetComponent<AudioSource>().PlayOneShot(_acDeath);
        StopAllCoroutines();
        _crubMode = -3;
        Destroy(gameObject,1.0f);
    }
}
