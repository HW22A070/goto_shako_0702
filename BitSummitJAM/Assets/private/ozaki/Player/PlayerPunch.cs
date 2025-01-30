using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
//using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField]
    [Tooltip("デバック用プレイヤーの色取得用")]
    private GameObject playerSkin;

    [SerializeField]
    [Tooltip("パンチポジション")]
    private GameObject punchPos;

    [SerializeField]
    [Tooltip("パンチ取得")]
    private GameObject punchHit;

    /// <summary>
    /// パンチをしているか
    /// </summary>
    private bool ispunch = false;

    [SerializeField]
    private float punchTime;

    private float punchnowTime = 0;

    [SerializeField]
    [Tooltip("パンチの方向")]
    private GameObject punchVectol;

    [SerializeField]
    [Tooltip("衝撃波")]
    private GameObject Shock;

    /// <summary>
    /// パンチ角度
    /// </summary>
    public float punchDegree;

    /// <summary>
    /// パンチチャージ時間
    /// </summary>
    private float chargeTime;

    public bool SonicPunch;

    [SerializeField]
    [Tooltip("パンチチャージ一段階目の時間")]
    private float punch1;

    [SerializeField]
    [Tooltip("パンチ力1")]
    private int punchPower1;

    [SerializeField]
    [Tooltip("パンチチャージ二段階目の時間")]
    private float punch2;

    [SerializeField]
    [Tooltip("パンチ力2")]
    private int punchPower2;

    [SerializeField]
    [Tooltip("パンチチャージ三段階目の時間")]
    private float punch3;

    [SerializeField]
    [Tooltip("パンチ力3")]
    private int punchPower3;

    /// <summary>
    /// パンチを打った時のパンチ力
    /// </summary>
    public int NowPunchPower;

    /// <summary>
    /// パンチチャージ
    /// </summary>
    public int punch;

    /// <summary>
    /// チャージをしているか
    /// </summary>
    public bool Charge;

    [Tooltip("パンチ角度")]
    private Vector3 punchRotate;

    [Tooltip("ジャンプ角度")]
    private Vector3 jumpRotate;

    /// <summary>
    /// パンチジャンプ取得
    /// </summary>
    private Jumper punchjump;

    /// <summary>
    /// パンチ用Ray
    /// </summary>
    private RaycastHit2D punchRayHit;

    private PlayerManager playerManager;

    /// <summary>
    /// PlayerSound取得
    /// </summary>
    private PlayerSoundC _cPlayerSound;

    [SerializeField, Tooltip("パンチエフェクト")]
    private PunchEffectC _prhbPunchEffect;

    [SerializeField, Tooltip("ダメージ")]
    private GameObject _damageEffect;

    [SerializeField]
    LayerMask mask;

    float range;

    [SerializeField]
    bool punchcool;

    [SerializeField]
    float cooltime;

    float nowcooltime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;

        Rigidbody2D rb = punchHit.GetComponent<Rigidbody2D>();

        //チャージ時間を初期化
        chargeTime = 0;

        //ジャンプ取得
        punchjump = GetComponent<Jumper>();

        //殴る角度初期化
        punchRotate = new Vector3(0f, 0f, 0f);

        //ジャンプ角度初期化
        jumpRotate = new Vector3(0f, 0f, 0f);

        punchHit.SetActive(false);

        punchVectol.SetActive(false);

        //PlayerSoundC取得
        _cPlayerSound = GetComponent<PlayerSoundC>();

        punchcool = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("joystick button 0") && !PlayerManager.IsPlayerMoveRock && !punchcool ||
            Input.GetKey("joystick button 1") && !PlayerManager.IsPlayerMoveRock && !punchcool)
        {
            PunchCharge();

            PunchRight();
        }

        if (1 <= punch && Input.GetKey("joystick button 4") && !PlayerManager.IsPlayerMoveRock)
        {
            Charge = true;
            //パンチの方向を決める
            PunchRotate();
        }

        if ((1 <= punch && Input.GetKeyUp("joystick button 0") ||
            1 <= punch && Input.GetKeyUp("joystick button 4") || 
            1 <= punch && Input.GetKeyUp("joystick button 1") && !PlayerManager.IsPlayerMoveRock))
        {

            Debug.Log("パンチ");

            PunchPowerScale();

            ispunch = true;

            punchHit.SetActive(true);

            PunchCheck();

            if ((Input.GetKeyUp("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f ||
                Input.GetKey("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f) && !PlayerManager.IsPlayerMoveRock)
            {
                ShockPunch();
            }

            chargeTime = 0;

            Charge = false;

            punch = 0;

            ChaegeReset();

            punchcool = true;
        }

        if (ispunch)
        {
            NowPunch();
        }

        if (PlayerManager.IsPlayerMoveRock) punch = 0;

        if(punchcool)
        {
            PunchCoolTime();
        }
    }

    private void PunchCharge()
    {
        chargeTime += Time.deltaTime;

        if (punch1 <= chargeTime && chargeTime < punch2)
        {
            if (punch != 1)
            {
                Instantiate(_prhbPunchEffect, punchPos.transform.position, transform.rotation).transform.parent = punchPos.transform;
                _cPlayerSound.SoundStart(0);
            }
            punch = 1;
        }

        else if (punch2 <= chargeTime && chargeTime < punch3)
        {
            if (punch != 2)
            {
                _cPlayerSound.SoundStart(1);
            }
            punch = 2;
        }

        else if (punch3 <= chargeTime)
        {
            if (punch != 3)
            {
                _cPlayerSound.SoundStart(2);
            }
            punch = 3;
        }

    }

    private void ChaegeReset()
    {
        chargeTime = 0;

        punch = 0;

        punchVectol.SetActive(false);

        if (GetComponent<PlayerManager>().IsRight)
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 0f, 0f);

            punchRotate = transform.right;
        }
        else
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 180f, 0f);

            punchRotate = -transform.right;
        }

    }

    private void PunchCheck()
    {
        Debug.Log("パンチ発射");

        _cPlayerSound.SoundStart(7);

        PunchRange();

        if (punchRayHit = Physics2D.Raycast(punchHit.transform.position,
                                            punchRotate,
                                            range))
        {
            Debug.Log("パンチ");
            Debug.Log(punchRayHit.collider.gameObject);
            if (punchRayHit.collider)
            {
                GameObject damageEffect = Instantiate(_damageEffect, punchHit.transform.position + new Vector3(-1, 1, 0), transform.localRotation);
                Destroy(damageEffect, 0.2f);

                if (punchRayHit.collider.gameObject.tag == "Ground" &&
                    Physics2D.Raycast(transform.position,
                                      -transform.up,
                                      transform.localScale.y * 0.5f, mask))
                {

                    Debug.Log("当たった");
                    if (punch != 1)
                    {
                        _cPlayerSound.SoundStart(8);
                    }

                    punchjump.Jumppunch = true;
                }
                else if ((punchRayHit.collider.gameObject.tag == "Enemy"))
                {
                    Debug.Log("敵に当たった" + NowPunchPower);

                    punchRayHit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(NowPunchPower);
                    _cPlayerSound.SoundStart(5);

                }
                else if ((punchRayHit.collider.gameObject.tag == "Gimmick"))
                {
                    Debug.Log("ギミック当たった");

                    var right = playerManager.Right();

                    punchRayHit.collider.gameObject.GetComponent<GimmickManager>().GimmickHit(NowPunchPower, right, punch);
                    _cPlayerSound.SoundStart(5);
                }


            }

        }
        Debug.DrawRay(punchHit.transform.position,
                punchRotate * range,
                Color.red,
                5);
    }

    /// <summary>
    /// パンチ回転
    /// </summary>
    private void PunchRotate()
    {
        punchVectol.SetActive(true);

        // 右スティック横入力
        var horizontal = Input.GetAxis("Horizontal");

        // 右スティック縦入力
        var vertical = Input.GetAxis("Vertical");

        // 右スティック入力角度を算出
        punchDegree = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

        // 角度をプラスの数字に
        if (punchDegree < 0f)
        {
            punchDegree += 360f;
        }

        if (horizontal == 0 && vertical == 0)
        {
            punchPos.SetActive(false);
        }
        else
        {
            punchPos.SetActive(true);
        }

        // 右スティックの入力があるとき
        if (horizontal != 0 || vertical != 0)
        {
            // 上半円の範囲で投げ軌道を変更
            // 投げる角度を代入
            punchRotate =
            new Vector3(Mathf.Cos((punchDegree) * Mathf.Deg2Rad), Mathf.Sin((punchDegree) * Mathf.Deg2Rad), 0f);
            // 投げ軌道を変更
            punchPos.transform.eulerAngles = new Vector3(0f, 0f, punchDegree);
        }

    }

    /// <summary>
    /// 衝撃波を放つ
    /// </summary>
    private void ShockPunch()
    {
        SonicPunch = true;
        // 生成と同時にオブジェクト情報を取得
        var shockObj = Instantiate(Shock, punchHit.transform.position, punchPos.transform.rotation);
        // オブジェクトからスクリプト情報を取得
        shockObj.GetComponent<ShockMove>().MoveDirection = punchPos.transform.right;

        _cPlayerSound.SoundStart(9);
    }

    /// <summary>
    /// パンチ力を取得する
    /// </summary>
    private void NowPunch()
    {
        punchnowTime += Time.deltaTime;

        if (punchTime <= punchnowTime)
        {
            ispunch = false;

            punchHit.SetActive(false);

            punchnowTime = 0;

            punchDegree = 0;

            if (GetComponent<PlayerManager>().IsRight)
            {
                punchPos.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else
            {
                punchPos.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }

            SonicPunch = false;


        }
    }

    public void PunchReset()
    {
        punch = 0;

        chargeTime = 0;

        punchVectol.SetActive(false);

        Charge = false;

        if (GetComponent<PlayerManager>().IsRight)
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            punchPos.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void PunchPowerScale()
    {
        switch (punch)
        {
            case 1:
                NowPunchPower = punchPower1;
                Debug.Log(NowPunchPower);
                break;
            case 2:
                NowPunchPower = punchPower2;
                Debug.Log(NowPunchPower);
                break;
            case 3:
                NowPunchPower = punchPower3;
                Debug.Log(NowPunchPower);
                break;
        }
    }


    public bool GraphicSender()
    {
        return ispunch;
    }

    public float GraphicSender2()
    {
        return punchDegree;
    }

    public int PunchLevelSender()
    {
        return punch;
    }

    private void PunchRight()
    {
        var hoge = playerManager.Right();

        if (hoge)
        {
            punchDegree = 0;
        }
        else
        {
            punchDegree = 180;
        }
    }

    public void PlayerOut()
    {
        Charge = false;

        punchVectol.SetActive(false);
    }

    void PunchRange()
    {
        switch (punch)
        {
            case 1:
                range = punchHit.transform.localScale.x * 0.7f;
                break;
            case 2:
                range = punchHit.transform.localScale.x * 2.0f;
                break;
            case 3:
                range = punchHit.transform.localScale.x * 3.5f;
                break;
        }
    }

    void PunchCoolTime()
    {
        nowcooltime += Time.deltaTime;

        if(cooltime <= nowcooltime)
        {
            punchcool = false;

            nowcooltime = 0;
        }
    }
}
