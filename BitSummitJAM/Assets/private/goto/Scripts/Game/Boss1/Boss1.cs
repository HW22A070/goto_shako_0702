using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    /// <summary>
    /// ボス行動モード
    /// 0=アイドル
    /// 1=戦闘
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
    /// 行動回数カウントアップ
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

    /// <summary>
    /// 影
    /// </summary>
    private Vector3 _positionOwnShadow;

    [SerializeField, Tooltip("衝撃波食らった時の1フレームの回転（540の約数で入力）")]
    private int _smashRotationValue = 30;

    /// <summary>
    /// 衝撃波食らった時のジャンプ
    /// </summary>
    private float _smashJumpValue;

    [SerializeField,Tooltip("ボスが反応するプレイヤーX")]
    private float _eyesight = 527;

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

    [SerializeField, Tooltip("影プレハブ")]
    private Shadow _shadowP;

    
    private Clear _cClear;

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
    private Coroutine _currentCoroutine,_attackCor;

    /// <summary>
    /// 重力
    /// </summary>
    private float _gravityScale = 0;

    [SerializeField, Tooltip("重力最大値、重力加速度")]
    private float _gravityMax = 0.02f, _gravityDelta = 0.01f;

    [SerializeField]
    private AudioClip _acDeath, _acDamage, _acBlocked,_acClow, _acPress,_acBabbleCharge,_acBabbleFire,_acSmashed, _acWakeUp,_acJamp;


    // Start is called before the first frame update
    void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");

        _enemyCoreScript = GetComponent<EnemyCoreC>();

        //GetComponent<GraphicC>().ResetAnimation(0);
        //無敵モードオン
        _enemyCoreScript._isGetBlowAble = false;
        //初期位置登録
        _positionOwnFirst = transform.localPosition;

        _cClear = GameObject.Find("ClearManager").GetComponent<Clear>();
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
        _boss1ToWallRayHitR = Physics2D.Raycast(_boss1Ray.origin + transform.right + (-transform.up * transform.localScale.y * 0.3f), _boss1Ray.direction, 10, 8);
        _boss1ToWallRayHitL = Physics2D.Raycast(_boss1Ray.origin + transform.right + (-transform.up * transform.localScale.y * 0.3f), _boss1Ray.direction, 10, 8);

        //床あたり判定（当たらなくなったら引き返し）
        _boss1ToFloorRayHit = Physics2D.Raycast(_boss1Ray.origin - (transform.up * transform.localScale.y*5.0f), _boss1Ray.direction, 10, 8);

        //生き埋め対策
        if (_boss1ToWallRayHitR && _boss1ToWallRayHitL && _boss1ToFloorRayHit&&_boss1Mode!=6)
        {
            transform.position += transform.up * 0.3f;
        }
        //床まで落ちる
        if (_boss1ToFloorRayHit)
        {
            _gravityScale = 0;
        }
        else
        {
            if (_boss1Mode < 2)
            {
                transform.position -= transform.up * _gravityScale;
            }
            //重力
            if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
            else _gravityScale = _gravityMax;
            Debug.Log("崖");
            TurnBack();
        }

        //通常時
        if (_boss1Mode == 0)
        {
            
            //発見
            if (_positionPlayerNow.x>= _eyesight)
            {
                Debug.Log("発見");
                _spriteYadokari.flipX = false;
                _boss1Mode = 1;
            }
        }
        //発見時
        if (_boss1Mode == 1)
        {
            if (_boss1Attack == 0)
            {
                //見失い
                if (_positionOwnNow.x - _eyeSight / 2 > _positionPlayerNow.x
                    || _gameobjectPlayer.transform.position.x > _positionOwnNow.x + _eyeSight / 2)
                {
                    Debug.Log("見失い");
                    //GetComponent<GraphicC>().ResetAnimation(0);
                    LookSet();
                    _boss1Mode = 0;
                }

                //攻撃継続
                else
                {
                    //HP351以上
                    if (_enemyCoreScript.HPCheck() > 350)
                    {
                        _bossCount++;
                        if (_bossCount == 1)
                        {
                            _boss1Attack = 1;
                            _bossCount = 0;
                        }
                        if (_bossCount++ >= 1) _bossCount = 0;
                    }
                    //HP201~350
                    else if (_enemyCoreScript.HPCheck() > 200)
                    {
                        _bossCount++;
                        if (_bossCount == 1)
                        {
                            _boss1Attack = 1;
                        }
                        else if (_bossCount == 2)
                        {
                            if (_positionOwnNow.x - _positionPlayerNow.x > 10) _boss1Attack = 2;
                            else _boss1Attack = 3;
                        }
                        else if (_bossCount == 3)
                        {
                            _boss1Attack = 1;
                        }
                        if (_bossCount >= 3) _bossCount = 0;

                    }
                    //HP51~250
                    else if(_enemyCoreScript.HPCheck() > 50)
                    {
                        _bossCount++;
                        if (_bossCount == 1)
                        {
                            _boss1Attack = 3;
                        }
                        else if (_bossCount == 2)
                        {
                            _boss1Attack = Random.Range(1, 4);
                        }
                        else if (_bossCount == 3)
                        {
                            _boss1Attack = Random.Range(1, 4);
                        }
                        if (_bossCount >= 3) _bossCount = 0;
                    }
                    //HP50以下
                    else
                    {
                        _bossCount++;
                        if (_bossCount == 1)
                        {
                            _boss1Attack = Random.Range(1, 4);
                            _bossCount = 0;
                        }
                        if (_bossCount++ >= 1) _bossCount = 0;
                    }
                    Debug.Log("ボスコード：" + _boss1Attack);
                    switch (_boss1Attack)
                    {
                        case 1:
                            _attackCor = StartCoroutine(Claws());
                            break;
                        case 2:
                            _attackCor = StartCoroutine(Babble());
                            break;
                        case 3:
                            _attackCor = StartCoroutine(Jump());
                            break;
                    }
                }
                
            }
        }


        //ふっとび大回転
        if (_boss1Mode == 3)
        {
            GetComponent<GraphicC>().ResetAnimation(11);
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
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            _spriteYadokari.flipY = false;
            _spriteYadokari.flipX = true;
            //走っていく
            transform.position += new Vector3(0.1f, -0.07f, 0);
        }

    }

    /// <summary>
    /// ツメ攻撃
    /// </summary>
    /// <returns></returns>
    private IEnumerator Claws()
    {
        GetComponent<GraphicC>().ResetAnimation(4);

        bool _isRight = _positionOwnNow.x <= _positionPlayerNow.x;
        _spriteYadokari.flipX = _isRight;
        if (_isRight)
        {
            for (int i = 0; i < 40; i++)
            {
                transform.position += transform.right * _walkSpeedChase * 2;
                yield return new WaitForSeconds(0.02f);
                if (_positionOwnNow.x+ _rayPositionDaray >= _positionPlayerNow.x) break;
            }
            GetComponent<GraphicC>().ResetAnimation(10);
            yield return new WaitForSeconds(2.00f);
            GetComponent<AudioSource>().PlayOneShot(_acClow);
            GetComponent<GraphicC>().ResetAnimation(2);
            _boss1Ray = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));
            _boss1ToPlayerRayHit = Physics2D.Raycast(_boss1Ray.origin - new Vector3(-_rayPositionDaray, 4.0f, 0), _boss1Ray.direction, 10, 128);
            Debug.DrawRay(_boss1Ray.origin +new Vector3(-_rayPositionDaray, 4.0f, 0), _boss1Ray.direction, Color.red, 0.5f);
        }
        else
        {
            for (int i = 0; i < 40; i++)
            {
                transform.position += -transform.right * _walkSpeedChase * 2;
                yield return new WaitForSeconds(0.02f);
                if (_positionOwnNow.x- _rayPositionDaray<= _positionPlayerNow.x) break;
            }
            GetComponent<GraphicC>().ResetAnimation(10);
            yield return new WaitForSeconds(2.00f);
            GetComponent<AudioSource>().PlayOneShot(_acClow);
            GetComponent<GraphicC>().ResetAnimation(2);
            _boss1Ray = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));
            _boss1ToPlayerRayHit = Physics2D.Raycast(_boss1Ray.origin - new Vector3(_rayPositionDaray, 4.0f, 0), _boss1Ray.direction, 10, 128);
            Debug.DrawRay(_boss1Ray.origin - new Vector3(_rayPositionDaray, 4.0f, 0), _boss1Ray.direction, Color.red, 0.5f);
        }

        
        if (_boss1ToPlayerRayHit.collider)
        {
            Debug.Log("ヤドカリこうげき！");
            _boss1ToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().PlayerDamage(1);
        }

        yield return new WaitForSeconds(0.02f*20);
        
        GetComponent<GraphicC>().ResetAnimation(0);
        _boss1Attack = 0;
    }

    /// <summary>
    /// 泡攻撃
    /// </summary>
    /// <returns></returns>
    private IEnumerator Babble()
    {
        GetComponent<GraphicC>().ResetAnimation(8);
        GetComponent<AudioSource>().PlayOneShot(_acBabbleCharge);
        bool _isRight = _positionOwnNow.x <= _positionPlayerNow.x;
        _spriteYadokari.flipX = _isRight;
        for (int i = 0; i < 25; i++)
        {
            if (_isRight)
            {
                Instantiate(_bubblePf, _positionOwnNow + new Vector3(2.0f, -4.0f, 0), transform.localRotation).EShot1(Random.Range(0, 360), 0.1f, 0.3f);
            }
            else
            {
                Instantiate(_bubblePf, _positionOwnNow + new Vector3(-2.0f, -4.0f, 0), transform.localRotation).EShot1(Random.Range(0, 360), 0.1f, 0.3f);
            }
           
            yield return new WaitForSeconds(0.02f * 2);

        }
        GetComponent<AudioSource>().PlayOneShot(_acBabbleFire);
        if(_isRight) Instantiate(_bressPf, _positionOwnNow + new Vector3(2.0f, -4.0f, 0), transform.localRotation).EShot1(0, 0.01f, 0.01f);
        else Instantiate(_bressPf, _positionOwnNow + new Vector3(-2.0f, -4.0f, 0), transform.localRotation).EShot1(180, 0.01f, 0.01f);

        yield return new WaitForSeconds(0.02f * 50);
        GetComponent<GraphicC>().ResetAnimation(0);
        _boss1Attack = 0;
    }

    /// <summary>
    /// ジャンプ攻撃
    /// </summary>
    /// <returns></returns>
    private IEnumerator Jump()
    {
        _spriteYadokari.flipX = false;
        GetComponent<GraphicC>().ResetAnimation(7);
        GetComponent<AudioSource>().PlayOneShot(_acJamp);
        Vector3 _positionOwnShadow = transform.position - (transform.up * 6.6f);
        //ジャンプ
        for(int i = 0; i < 100; i++)
        {
            transform.position += transform.up * 1.5f;
            yield return new WaitForSeconds(0.02f);
        }

        //軸合わせ
        for (int i = 0; i < 10; i++)
        {
            Chase(0, 5.0f);
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<GraphicC>().ResetAnimation(2);

        _positionOwnShadow.x = transform.position.x;
        Shadow _shadowInst= Instantiate(_shadowP, _positionOwnShadow, transform.localRotation);
        _shadowInst.Summoned(1.5f);
        //降下
        do
        {
            _boss1ToFloorRayHit = Physics2D.Raycast(_boss1Ray.origin - (transform.up * transform.localScale.y * 5.0f), _boss1Ray.direction, 10, 8);
            transform.position -= transform.up * 2.0f;

            _boss1Ray = new Ray(_positionOwnNow, new Vector3(10, 0, 0));
            _boss1ToPlayerRayHit = Physics2D.Raycast(_boss1Ray.origin - (transform.up * transform.localScale.y * 6.0f)-(transform.right*5.0f), _boss1Ray.direction, 10.0f, 128);
            Debug.DrawRay(_boss1Ray.origin - (transform.up * transform.localScale.y * 6.0f) - (transform.right * 5.0f), transform.right * 10, Color.red, 3.0f);
            if (_boss1ToPlayerRayHit.collider)
            {
                _boss1ToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().PlayerDamage(1);
            }

            yield return new WaitForSeconds(0.02f);
        }
        while (!_boss1ToFloorRayHit);
        GetComponent<AudioSource>().PlayOneShot(_acPress);
        while (_boss1ToFloorRayHit)
        {
            _boss1Ray = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));
            _boss1ToFloorRayHit = Physics2D.Raycast(_boss1Ray.origin - transform.up * _rayPositionDaray, _boss1Ray.direction, 10, 8);
            transform.position += transform.up * 0.5f;
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.02f * 50);

        GetComponent<GraphicC>().ResetAnimation(0);
        _boss1Attack = 0;
    }

    /// <summary>
    /// プレイヤー追いかけ行動
    /// </summary>
    private void Chase(float deray, float speed)
    {
        //プレイヤーおいかける
        if (_positionOwnNow.x - deray <= _positionPlayerNow.x)
        {
            _spriteYadokari.flipX = true;
            transform.position += transform.right * speed;
        }
        else
        {
            _spriteYadokari.flipX = false;
            transform.position -= transform.right * speed;
        }


        //座標Uターン
        if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
            || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
        {
            TurnBack();
        }
        if (_boss1ToWallRayHitR || _boss1ToWallRayHitL)
        {
            TurnBack();
        }

    }

    private void AllAttackCorotineStop()
    {
        _bossCount = 0;
        StopCoroutine(Claws());
        StopCoroutine(Babble());
        StopCoroutine(Jump());
        _attackCor = null;
    }


    /// <summary>
    /// 衝撃波食らってひっくりかえる
    /// </summary>
    public void Smashed()
    {
        if (_boss1Mode == 1&&_bossCount!=3)
        {
            _sonicHp--;
        }
        if (_sonicHp <= 0)
        {
            //通常か移動中なら衝撃波効果あり
            if (0 <= _boss1Mode && _boss1Mode <= 1)
            {
                _spriteYadokari.flipX = false;
                GetComponent<AudioSource>().PlayOneShot(_acSmashed);
                AllAttackCorotineStop();
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
        yield return new WaitForSeconds(9.0f);
        GetComponent<AudioSource>().PlayOneShot(_acWakeUp);
        _boss1Mode = 5;
    }

    /// <summary>
    /// 被ダメージ
    /// </summary>
    public void GetDamage()
    {
        if (_boss1Mode == 4)
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
        GetComponent<AudioSource>().PlayOneShot(_acDeath);
        transform.localEulerAngles = Vector3.zero;
        GetComponent<GraphicC>().ResetAnimation(1);
        StopAllCoroutines();
        _currentCoroutine = null;
        _boss1Mode = 6;
        StartCoroutine(DeathMove());
        
    }

    private IEnumerator DeathMove()
    {
        yield return new WaitForSeconds(5.0f);
        _cClear.ClearEffect();
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
