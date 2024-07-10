using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("プレイヤー最大Hp")]
    private int maxHp;
    /// <summary>
    /// プレイヤー最大Hp
    /// </summary>
    public int MaxHp
    {
        get => maxHp;
    }
    [SerializeField]
    [Tooltip("プレイヤーHp")]
    private int hp;
    /// <summary>
    /// プレイヤー最大Hp
    /// </summary>
    public int Hp
    {
        get => hp;
        set => hp = value;
    }

    [SerializeField]
    [Tooltip("プレイヤーが右を向いているか")]
    private bool isRight;

    private bool isRightNow;

    public bool IsRight
    {
        get => isRight;
        set => isRight = value;
    }

    private bool isRightnow;

    [SerializeField]
    private float _Time = 0;

    private float now = 0;

    private bool Hit = false;

    [SerializeField]
    private GameObject playerBody;

    [SerializeField]
    private GameObject punchPos;

    // Start is called before the first frame update
    void Start()
    {
        isRight = true;

        isRightnow = isRight;

        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            isRight = true;
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            isRight = false;
        }

        if(isRight != isRightNow)
        {
            IsRightChange();

            isRightNow = isRight;
        }

        if (Hit)
        {
            now += Time.deltaTime;

            if (_Time <= now)
            {
                Hit = false;

                now = 0;
            }
        }

    }

    public void PlayerDamage(int hoge)
    {
        if (Hit == false)
        {
            Hit = true;

            hp -= hoge;
        }

        GetComponent<PlayerPunch>().PunchReset();

        if (hp <= 0)
        {
            Debug.Log("ゲームオーバー");
            SceneManagementC.LoadScene("GameOverScene");
        }
    }

    public void IsRightChange()
    {
        if (IsRight == true)
        {
            playerBody.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        }
        else
        {
            playerBody.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    public bool GraphicSender()
    {
        return Hit;
    }

    public int HPSender()
    {
        return Hp;
    }
}
