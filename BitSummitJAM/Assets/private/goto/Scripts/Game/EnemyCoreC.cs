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
    [Tooltip("�G��HP")]
    private int _enemyHp;

    /// <summary>
    /// �p���`��������
    /// </summary>
    public bool _isGetBlowAble = true;

    /// <summary>
    /// �Ռ��g��������
    /// </summary>
    public bool _isGetSonicAble = true;

    /// <summary>
    /// ����ł��邩�B���񂾂�true
    /// </summary>
    private bool _isDead;

    /// <summary>
    /// �����̍��W
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// ��_�����邩
    /// </summary>
    [DoNotSerialize]
    public bool IsGetDamage;

    // Start is called before the first frame update
    void Start()
    {

    }


    private void Update()
    {
        //�f�o�b�O�p�BO�������ƃp���`1�_���[�W�󂯂�
        if (Input.GetKeyDown(KeyCode.O))
        {
            GetBlow(1);
        }
        //�f�o�b�O�p�BP�������ƏՌ��g���󂯂�
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetSonic();
        }

        //�f�o�b�O�p�BI�������Ɩⓚ���p�œG���S������
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
    /// �p���`�U�����󂯂�
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
    /// �ׂ����
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
    /// �Ռ��g���󂯂�
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
    /// ����
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

    [Header("0=�p���` 1=�Ռ��g 2=��")]
    public List<UnityEvent> OnEvent;

    /// <summary>
    /// 
    /// ���炩�̏Ռ���H������Ƃ��̓G���Ƃ̍s��
    /// 0=�p���`
    /// 1=�Ռ��g
    /// 2=���s��
    /// 3=����
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