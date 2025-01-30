using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YadokariC : MonoBehaviour
{

    /// <summary>
    /// ���h�J���s�����[�h
    /// 0=�ʏ펞
    /// 1=����
    /// 2=�U��
    /// 3=�ӂ��Ƃђ�
    /// 4=���˂��h����
    /// 5=���A��
    /// 6=�����i���j�j
    /// 7=����
    /// </summary>
    /// [SerializeField]
    private int _yadokariMode=0;

    /// <summary>
    /// �X�^�[�g�㒅�n������
    /// </summary>
    private bool _isSet;

    /// <summary>
    /// �v���C���[���
    /// </summary>
    private GameObject _gameobjectPlayer;

    /// <summary>
    /// �����̍��W
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// �����̏����ʒu
    /// </summary>
    private Vector3 _positionOwnFirst;

    /// <summary>
    /// Player�̍��W
    /// </summary>
    private Vector3 _positionPlayerNow;

    [SerializeField,Tooltip("�Ռ��g�H���������1�t���[���̉�]�i540�̖񐔂œ��́j")]
    private int _smashRotationValue=30;

    /// <summary>
    /// �Ռ��g�H��������̃W�����v
    /// </summary>
    private float _smashJumpValue;

    /// <summary>
    /// �Ռ��g�H��������̊��藦
    /// </summary>
    private int _smashCount = 0;

    [SerializeField]
    [Tooltip("���������s�X�s�[�h(cm/f)")]
    private float _walkSpeed;

    [SerializeField]
    [Tooltip("U�^�[������ړ�����(0������Ɩ����ɐi��)")]
    private float _turnBackDistance;

    [SerializeField]
    [Tooltip("���G�͈͑傫���i���vpixel�j")]
    private float _eyeSight;

    [SerializeField]
    [Tooltip("�ǔ����s�X�s�[�h(cm/f)")]
    private float _walkSpeedChase;

    /// <summary>
    /// ���h�J�����C�L���X�g��`
    /// </summary>
    private Ray _yadokariRay;

    /// <summary>
    /// ���h�J��to�v���C���[�Փ˔���
    /// </summary>
    private RaycastHit2D _yadokariToPlayerRayHit;

    /// <summary>
    /// ���h�J��to�ǏՓ˔���
    /// </summary>
    private RaycastHit2D _yadokariToWallRayHitR, _yadokariToWallRayHitL;

    /// <summary>
    /// ���h�J��to���Փ˔���
    /// </summary>
    private RaycastHit2D _yadokariToFloorRayHit;

    /// <summary>
    /// GraphicC�o�^
    /// </summary>
    private GraphicC _graphicScrpt;

    /// <summary>
    /// ECoreC�o�^
    /// </summary>
    private EnemyCoreC _enemyCoreScript;

    [SerializeField]
    [Tooltip("���h�J���X�v���C�g�ύX��")]
    private SpriteRenderer _spriteYadokari;

    /// <summary>
    /// ���s�R���[�`��
    /// </summary>
    private Coroutine _currentCoroutine;

    /// <summary>
    /// �d��
    /// </summary>
    private float _gravityScale = 0;

    [SerializeField, Tooltip("�d�͍ő�l�A�d�͉����x")]
    private float _gravityMax=0.02f, _gravityDelta=0.01f;

    [SerializeField]
    private AudioClip _acDeath, _acDamage, _acBlocked,_acClow, _acSmashed,_acWakeUp,_acStinging;


    // Start is called before the first frame update
    void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");

        //GetComponent<GraphicC>().ResetAnimation(0);
        //���G���[�h�I��
        GetComponent<EnemyCoreC>()._isGetBlowAble = false;
        //�����ʒu�o�^
        _positionOwnFirst = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //���݈ʒu�X�V
        _positionOwnNow = transform.localPosition;
        //�v���C���[�ʒu����
        _positionPlayerNow = _gameobjectPlayer.transform.position;
    }

    void FixedUpdate()
    {
        _yadokariRay = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));

        //�ǂ����蔻��i��������������Ԃ��j
        _yadokariToWallRayHitR = Physics2D.Raycast(_yadokariRay.origin + (transform.right * 0.5f) + (-transform.up * transform.localScale.y * 0.3f), _yadokariRay.direction, 10, 8);
        _yadokariToWallRayHitL = Physics2D.Raycast(_yadokariRay.origin - (transform.right * 0.5f) + (-transform.up * transform.localScale.y * 0.3f), _yadokariRay.direction, 10, 8);
        //�������蔻��i������Ȃ��Ȃ���������Ԃ��j
        _yadokariToFloorRayHit = Physics2D.Raycast(_yadokariRay.origin - (transform.up * transform.localScale.y * 1.0f), _yadokariRay.direction, 10, 8);

        //�ǂɋ��܂ꂽ�疄�܂��Ă���Ƃ݂Ȃ������オ��(����łȂ��Ƃ�����)
        if (_yadokariToWallRayHitR && _yadokariToWallRayHitL && _yadokariToFloorRayHit&&_yadokariMode<6)
        {
            transform.position += transform.up * 0.2f;
        }

        //�ʏ펞
        if (_yadokariMode == 0)
        {
            //�ړ�
            transform.position += transform.right * _walkSpeed;

            //���WU�^�[��
            /*if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
            {
                TurnBack();
            }*/

            if (_yadokariToWallRayHitR || _yadokariToWallRayHitL)
            {
                TurnBack();
            }

            //���܂ŗ�����
            if (_yadokariToFloorRayHit)
            {
                _isSet = true;
                _gravityScale = 0;
            }
            else
            {
                transform.position -= transform.up * _gravityScale;
                //�d��
                if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
                else _gravityScale = _gravityMax;
                Debug.Log("�R");
                //�X�^�[�g�㒅�n����܂ŉ�]���Ȃ��B�v���v�������Ⴄ
                if (_isSet)
                {
                    TurnBack();
                }
            }

            //����
            if (_positionOwnNow.x - _eyeSight / 2 <= _positionPlayerNow.x
                && _gameobjectPlayer.transform.position.x <= _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("����");
                GetComponent<GraphicC>().ResetAnimation(10);
                _yadokariMode = 1;
            }
        }

        //�ǔ�
        if (_yadokariMode == 1)
        {
            //�v���C���[��������
            if (_positionOwnNow.x <= _positionPlayerNow.x)
            {

                _spriteYadokari.flipX = true;
                //�ǂ��R���Ȃ���ΐi��
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
                //�ǂ��R���Ȃ���ΐi��
                if (!_yadokariToWallRayHitR && !_yadokariToWallRayHitL&& _yadokariToFloorRayHit)
                {
                    transform.position -= transform.right * _walkSpeedChase;
                }
                else
                {
                    transform.position += transform.right * _walkSpeedChase * 10;
                }
            }

            //���܂ŗ�����
            if (_yadokariToFloorRayHit)
            {
                _gravityScale = 0;
            }
            else
            {
                transform.position -= transform.up * _gravityScale;
                //�d��
                if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
                else _gravityScale = _gravityMax;
                Debug.Log("�R");
            }

            //�v���C���[�����蔻��i����������U���j
            _yadokariToPlayerRayHit = Physics2D.Raycast(_yadokariRay.origin, _yadokariRay.direction, 10, 128);
            if (_yadokariToPlayerRayHit.collider)
            {
                _yadokariMode = 2;
                _currentCoroutine= StartCoroutine(AttackAnimation(_yadokariToPlayerRayHit.collider.gameObject));
            }

            //������
            if (_positionOwnNow.x - _eyeSight / 2 > _positionPlayerNow.x
                || _gameobjectPlayer.transform.position.x > _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("������");
                GetComponent<GraphicC>().ResetAnimation(0);
                LookSet();
                _yadokariMode = 0;
            }
        }
        

        //�ӂ��Ƃё��]
        if (_yadokariMode == 3)
        {
            transform.localEulerAngles += transform.forward * -_smashRotationValue;
            transform.localPosition += new Vector3(0, _smashJumpValue, 0);
            _smashJumpValue-=0.06f;
            _smashCount++;
            //���]��������˂��h����
            if (_smashCount>=540/_smashRotationValue)
            {
                Debug.Log("������I�I�I");
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
                Debug.Log("���ǂ�I�I�I");
                transform.localPosition += new Vector3(0, _smashJumpValue, 0);
                GetComponent<EnemyCoreC>()._isGetBlowAble = false;
                _smashCount = 0;
                _smashJumpValue = 0;
                GetComponent<GraphicC>().ResetAnimation(0);

                _yadokariMode = 0;
            }
        }

        //���s��
        else if (_yadokariMode==6)
        {
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            transform.localEulerAngles = Vector3.zero;
            _spriteYadokari.flipY = false;
            _spriteYadokari.flipX = true;
            //�����Ă���
            transform.position += new Vector3(0.01f, -0.01f, 0);
        }

        //�����s��
        else if (_yadokariMode == 7)
        {
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            transform.localEulerAngles = Vector3.zero;
            _spriteYadokari.flipY = false;
            _spriteYadokari.flipX = true;
        }

    }

    /// <summary>
    /// �U��
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    private IEnumerator AttackAnimation(GameObject targetPlayer)
    {
        GetComponent<GraphicC>().ResetAnimation(2);
        GetComponent<AudioSource>().PlayOneShot(_acClow);
        Debug.Log("���h�J�����������I");
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().name);
        //Debug.Log(targetPlayer.GetComponentInParent<PlayerManager>().Hp);
        targetPlayer.GetComponentInParent<PlayerManager>().PlayerDamage(1);
        yield return new WaitForSeconds(0.5f);
        GetComponent<GraphicC>().ResetAnimation(10);
        _yadokariMode = 1;
    }

    /// <summary>
    /// �Ռ��g�H����ĂЂ����肩����
    /// </summary>
    public void Smashed()
    {
        //�ʏ킩�ړ����Ȃ�Ռ��g���ʂ���
        if (0 <= _yadokariMode && _yadokariMode <= 1)
        {
            GetComponent<AudioSource>().PlayOneShot(_acSmashed);
            GetComponent<GraphicC>().ResetAnimation(11);
            _yadokariMode = 3;
            _smashJumpValue = (540 / _smashRotationValue) * 0.03f;
        }
    }

    /// <summary>
    /// ���˂��h����A�N�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownAction()
    {
        yield return new WaitForSeconds(3.0f);
        _yadokariMode = 5;
        GetComponent<AudioSource>().PlayOneShot(_acWakeUp);
    }
    
    /// <summary>
    /// ��_���[�W
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
    /// ���A�N�V�����X�^�[�g
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
    /// ����
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
    /// �ǊR�����]��
    /// </summary>
    private void TurnBack()
    {
        transform.position -= transform.right * _walkSpeed * 5;
        _walkSpeed *= -1;
        LookSet();
    }

    /// <summary>
    /// ���E�������킹
    /// </summary>
    private void LookSet()
    {
        if (_walkSpeed < 0) _spriteYadokari.flipX = false;
        else _spriteYadokari.flipX = true;
    }

    private void OnBecameInvisible()
    {
        //�ʏ펞�ɉ�ʂ̊O�ɂ�����x�~
        if (_yadokariMode == 0) _yadokariMode = 999;
    }

    private void OnBecameVisible()
    {
        //�x�~���ɉ�ʂ̒��ɂ�����ĊJ
        if (_yadokariMode == 999) _yadokariMode = 0;
    }

}
