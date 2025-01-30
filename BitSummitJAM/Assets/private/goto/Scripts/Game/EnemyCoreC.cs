using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Events;

public class EnemyCoreC : MonoBehaviour
{
    [SerializeField]
    [Tooltip("敵のHP")]
    private int _enemyHp;

    /// <summary>
    /// パンチが効くか
    /// </summary>
    public bool _isGetBlowAble = true;

    /// <summary>
    /// 衝撃波が効くか
    /// </summary>
    public bool _isGetSonicAble = true;

    /// <summary>
    /// 死んでいるか。死んだらtrue
    /// </summary>
    private bool _isDead;

    /// <summary>
    /// 自分の座標
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// 被ダメあるか
    /// </summary>
    [DoNotSerialize]
    public bool IsGetDamage;

    // Start is called before the first frame update
    void Start()
    {

    }


    private void Update()
    {
        //デバッグ用。Oを押すとパンチ1ダメージ受ける
        if (Input.GetKeyDown(KeyCode.O))
        {
            GetBlow(1);
        }
        //デバッグ用。Pを押すと衝撃波を受ける
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetSonic();
        }

        //デバッグ用。Iを押すと問答無用で敵が全員死ぬ
        if (Input.GetKeyDown(KeyCode.I))
        {
            GetBlow(9999);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _positionOwnNow = transform.position;
    }

    /// <summary>
    /// パンチ攻撃を受ける
    /// </summary>
    public void GetBlow(int attackPower)
    {
        if (!_isDead)
        {
            DamageAction(0);
            if (_isGetBlowAble)
            {
                _enemyHp -= attackPower;
                if (_enemyHp <= 0)DeathStart();
            }
            else
            {
            }
        }
    }

    /// <summary>
    /// 潰される
    /// </summary>
    public void GetScrap()
    {
        if (!_isDead)
        {
            DamageAction(3);
            _enemyHp =0;
            _isDead = true;
        }
    }



    /// <summary>
    /// 衝撃波を受ける
    /// </summary>
    public void GetSonic()
    {
        DamageAction(1);
        if (_isGetSonicAble == true && _isDead == false)
        {
            if (IsGetDamage)
            {
                //_enemyHp -= attackPower;
                if (_enemyHp <= 0)
                {
                    DeathStart();
                }
            }
            else
            {
            }
        }
    }

    /// <summary>
    /// 死ぬ
    /// </summary>
    private void DeathStart()
    {
        DamageAction(2);
        _isDead = true;
        
    }

    public int HPCheck()
    {
        return _enemyHp;
    }

    [Header("0=パンチ 1=衝撃波 2=死")]
    public List<UnityEvent> OnEvent;

    /// <summary>
    /// 
    /// 何らかの衝撃を食らったときの敵ごとの行動
    /// 0=パンチ
    /// 1=衝撃波
    /// 2=死行動
    /// 3=圧死
    /// </summary>
    /// <param name="id"></param>
    public void DamageAction(int id)
    {
        OnEvent[id].Invoke();
    }
}

#if UNITY_EDITOR
[ExecuteAlways]
[CustomEditor(typeof(EnemyCoreC))]
public class MethodCallerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyCoreC t = target as EnemyCoreC;
        if (t == null) return;
        if (t.OnEvent == null) return;
        if (t.OnEvent.Count > 1)
        {
            for (int i = 0; i < t.OnEvent.Count; i++)
            {
                if (t.OnEvent[i].GetPersistentEventCount() > 0 && t.OnEvent[i].GetPersistentMethodName(0).Length > 0)
                {
                    if (GUILayout.Button(t.OnEvent[i].GetPersistentMethodName(0)))
                    {
                        t.DamageAction(i);
                    }
                }
            }
        }
    }
}
#endif