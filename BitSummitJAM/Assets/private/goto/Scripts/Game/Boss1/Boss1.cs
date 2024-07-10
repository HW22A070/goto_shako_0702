using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    /// <summary>
    /// �{�X�s�����[�h
    /// 0=�ʏ펞
    /// 1=����
    /// 2=�U��
    /// 3=�ӂ��Ƃђ�
    /// 4=���˂��h����
    /// 5=���A��
    /// 6=�����i���j�j
    /// </summary>
    private int _boss1Mode;

    /// <summary>
    /// �{�X�U�����[�h
    /// 0=�Ȃ�
    /// 1=�c��
    /// </summary>
    private int _boss1Attack;

    /// <summary>
    /// �s���J�E���g�A�b�v
    /// </summary>
    private int _bossCount = 0;

    [SerializeField]
    [Tooltip("�v���C���[���")]
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

    [SerializeField, Tooltip("�Ռ��g�H���������1�t���[���̉�]�i540�̖񐔂œ��́j")]
    private int _smashRotationValue = 30;

    /// <summary>
    /// �Ռ��g�H��������̃W�����v
    /// </summary>
    private float _smashJumpValue;

    /// <summary>
    /// �Ռ��g�H��������̊��藦
    /// </summary>
    private int _smashCount = 0;

    [SerializeField,Tooltip("Ray�f�B���C")]
    private float _rayPositionDaray;

    [SerializeField,Tooltip("���������s�X�s�[�h(cm/f)")]
    private float _walkSpeed;

    [SerializeField,Tooltip("U�^�[������ړ�����(0������Ɩ����ɐi��)")]
    private float _turnBackDistance;

    [SerializeField,Tooltip("���G�͈͑傫���i���vpixel�j")]
    private float _eyeSight;

    [SerializeField,Tooltip("�ǔ����s�X�s�[�h(cm/f)")]
    private float _walkSpeedChase;

    [SerializeField,Tooltip("�u���X�v���n�u")]
    private BressC _bressPf;

    [SerializeField, Tooltip("EXP�v���n�u")]
    private ExpC _bubblePf;

    /// <summary>
    /// ���h�J�����C�L���X�g��`
    /// </summary>
    private Ray _boss1Ray;

    /// <summary>
    /// ���h�J��to�v���C���[�Փ˔���
    /// </summary>
    private RaycastHit2D _boss1ToPlayerRayHit;

    /// <summary>
    /// ���h�J��to�ǏՓ˔���
    /// </summary>
    private RaycastHit2D _boss1ToWallRayHitR, _boss1ToWallRayHitL;

    /// <summary>
    /// ���h�J��to���Փ˔���
    /// </summary>
    private RaycastHit2D _boss1ToFloorRayHit;

    /// <summary>
    /// �Ռ��g�Ђ����肩����J�E���g
    /// </summary>
    private int _sonicHp=3;

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
    private float _gravityMax = 0.02f, _gravityDelta = 0.01f;


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
        _boss1Ray = new Ray(_positionOwnNow, new Vector3(0, 0, Mathf.Infinity));

        //�ǂ����蔻��i��������������Ԃ��j
        _boss1ToWallRayHitR = Physics2D.Raycast(_boss1Ray.origin + new Vector3(-_rayPositionDaray, -4.0f, 0), _boss1Ray.direction, 10, 8);
        _boss1ToWallRayHitL = Physics2D.Raycast(_boss1Ray.origin + new Vector3(_rayPositionDaray, -4.0f, 0), _boss1Ray.direction, 10, 8);

        //�������蔻��i������Ȃ��Ȃ���������Ԃ��j
        _boss1ToFloorRayHit = Physics2D.Raycast(_boss1Ray.origin - transform.up * _rayPositionDaray, _boss1Ray.direction, 10, 8);
        //���܂ŗ�����
        if (_boss1ToFloorRayHit)
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
            TurnBack();
        }

        //�ʏ펞
        if (_boss1Mode == 0)
        {
            //�ړ�
            transform.position += transform.right * _walkSpeed;

            //���WU�^�[��
            if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
            {
                TurnBack();
            }

            //���C��`
            if (_boss1ToWallRayHitR || _boss1ToWallRayHitL)
            {
                TurnBack();
            }
            

            //����
            if (_positionOwnNow.x - _eyeSight / 2 <= _positionPlayerNow.x
                && _gameobjectPlayer.transform.position.x <= _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("����");
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
                    Debug.Log("�{�X�R�[�h�F" + _boss1Attack);
                    _bossCount = 0;
                }
            }

            //�c��
            if (_boss1Attack == 1)
            {
                GetComponent<GraphicC>().ResetAnimation(10);
                //�v���C���[��������
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

                //���WU�^�[��
                if ((_positionOwnNow.x >= _positionOwnFirst.x + _turnBackDistance
                    || _positionOwnNow.x < _positionOwnFirst.x) && _turnBackDistance != 0)
                {
                    TurnBack();
                }

                //���C��`
                if (_boss1ToWallRayHitR || _boss1ToWallRayHitL)
                {
                    TurnBack();
                }



                if (_boss1Attack == 1)
                {
                    //�v���C���[�����蔻��i����������U���j
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

            //�A
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


            //������
            /*
            if (_positionOwnNow.x - _eyeSight / 2 > _positionPlayerNow.x
                || _gameobjectPlayer.transform.position.x > _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("������");
                //GetComponent<GraphicC>().ResetAnimation(0);
                LookSet();
                _boss1Mode = 0;
            }
            */
        }


        //�ӂ��Ƃё��]
        if (_boss1Mode == 3)
        {
            transform.localEulerAngles += transform.forward * -_smashRotationValue;
            transform.localPosition += new Vector3(0, _smashJumpValue, 0);
            _smashJumpValue -= 0.06f;
            _smashCount++;
            //���]��������˂��h����
            if (_smashCount >= 540 / _smashRotationValue)
            {
                Debug.Log("������I�I�I");
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
                Debug.Log("���ǂ�I�I�I");
                transform.localPosition += new Vector3(0, _smashJumpValue, 0);
                GetComponent<EnemyCoreC>()._isGetBlowAble = false;
                _smashCount = 0;
                _smashJumpValue = 0;
                GetComponent<GraphicC>().ResetAnimation(0);
                _sonicHp = 3;
                _boss1Mode = 0;
            }
        }

        //���s��
        else if (_boss1Mode == 6)
        {
            _spriteYadokari.flipY = true;
            //_deathMoveSpeed -= _deathMoveDeltaY;
            //�Ђ�����Ԃ��ė����Ă䂭
            transform.position += new Vector3(0, 0.01f, 0);
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
        Debug.Log("���h�J�����������I");
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
    /// �Ռ��g�H����ĂЂ����肩����
    /// </summary>
    public void Smashed()
    {
        if(_boss1Attack == 1)
        {

            _sonicHp--;
        }
        if (_sonicHp <= 0)
        {
            //�ʏ킩�ړ����Ȃ�Ռ��g���ʂ���
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
    /// ���˂��h����A�N�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownAction()
    {
        yield return new WaitForSeconds(15.0f);
        _boss1Mode = 5;
    }

    /// <summary>
    /// ��_���[�W
    /// </summary>
    public void GetDamage()
    {
        if (_boss1Mode == 4)
        {
            Debug.Log("�k���I");
            //StartCoroutine(KnockBack());
        }
        else
        {
            Debug.Log("�k�Ɏ��ꂽ���Ƀp���`�͌����Ȃ��˂�");
        }
    }

    /// <summary>
    /// ���A�N�V�����X�^�[�g
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
    /// �ǊR�����]��
    /// </summary>
    private void TurnBack()
    {
        transform.position += -transform.right * _walkSpeed * 2;
        _walkSpeed *= -1;
        //LookSet();
    }

    /// <summary>
    /// ���E�������킹
    /// </summary>
    private void LookSet()
    {
        if (_walkSpeed < 0) _spriteYadokari.flipX = false;
        else _spriteYadokari.flipX = true;
    }
}
