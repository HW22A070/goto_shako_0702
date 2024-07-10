using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private float punchDegree;

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

    private Renderer playerColor;

    /// <summary>
    /// パンチジャンプ取得
    /// </summary>
    private Jumper punchjump;

    /// <summary>
    /// パンチ用Ray
    /// </summary>
    private RaycastHit2D punchRayHit;

    // Start is called before the first frame update
    void Start()
    {
        //チャージ時間を初期化
        chargeTime = 0;

        //色を取得
        playerColor = playerSkin.GetComponent<Renderer>();

        //ジャンプ取得
        punchjump = GetComponent<Jumper>();

        //殴る角度初期化
        punchRotate = new Vector3(0f, 0f, 0f);

        //ジャンプ角度初期化
        jumpRotate = new Vector3(0f, 0f, 0f);

        punchHit.SetActive(false);

        punchVectol.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("joystick button 0"))
        {
            PunchCharge();
        }

        if (1 <= punch && Input.GetKey("joystick button 4"))
        {
            Charge = true;
            //パンチの方向を決める
            PunchRotate();
        }

        if (1 <= punch && Input.GetKeyUp("joystick button 0") || 1 <= punch && Input.GetKeyUp("joystick button 4"))
        {

            Debug.Log("パンチ");

            PunchPowerScale();

            ispunch = true;

            punchHit.SetActive(true);

            PunchCheck();

            if (Input.GetKeyUp("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f ||
                Input.GetKey("joystick button 4") && punchDegree >= 0f && punchDegree <= 200f)
            {
                ShockPunch();
            }

            chargeTime = 0;

            Charge = false;

            punch = 0;

            playerColor.material.color = Color.white;

            ChaegeReset();
        }

        if(ispunch)
        {
            NowPunch();
        }
    }

    private void PunchCharge()
    {
        chargeTime += Time.deltaTime;

        if (punch1 <= chargeTime && chargeTime < punch2)
        {
            punch = 1;
        }

        else if (punch2 <= chargeTime && chargeTime < punch3)
        {
            punch = 2;
        }

        else if (punch3 <= chargeTime)
        {
            punch = 3;
        }

        switch (punch)
        {

            case 0:
                playerColor.material.color = Color.white;
                break;

            case 1:
                playerColor.material.color = Color.red;
                break;

            case 2:
                playerColor.material.color = Color.green;
                break;
            case 3:
                playerColor.material.color = Color.blue;
                break;

        }
    }

    private void ChaegeReset()
    {
        chargeTime = 0;

        punch = 0;

        playerColor.material.color = Color.white;

        punchVectol.SetActive(false);
    }

    private void PunchCheck()
    {
        Debug.Log("パンチ発射");

        if (punchRayHit = Physics2D.Raycast(punchHit.transform.position,
                                            punchRotate,
                                            punchHit.transform.localScale.x * 0.5f,511))
        {
            Debug.Log("パンチ");
            Debug.Log(punchRayHit.collider.gameObject);
            if (punchRayHit.collider)
            {
                if (punchRayHit.collider.gameObject.tag == "Ground")
                {
                    Debug.Log("当たった");
                    punchjump.Jumppunch = true;
                }

                else if ((punchRayHit.collider.gameObject.tag == "Enemy"))
                {
                    Debug.Log("敵に当たった"+ NowPunchPower);

                    punchRayHit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(NowPunchPower);

                }

                else if ((punchRayHit.collider.gameObject.tag == "Gimmick"))
                {
                    Debug.Log("ギミック当たった");

                    punchRayHit.collider.gameObject.GetComponent<GimmickManager>().GimmickHit(NowPunchPower,true,punch);
                }
            }

        }
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
    }

    /// <summary>
    /// パンチ力を取得する
    /// </summary>
    private void NowPunch()
    {
        punchnowTime += Time.deltaTime;

        if(punchTime <= punchnowTime)
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

}
