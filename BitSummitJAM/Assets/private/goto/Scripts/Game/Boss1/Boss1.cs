using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    /// <summary>
    /// �{�X�s�����[�h
    /// 0=�A�C�h��
    /// 1=�퓬
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
    /// �s���񐔃J�E���g�A�b�v
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

    /// <summary>
    /// �e
    /// </summary>
    private Vector3 _positionOwnShadow;

    [SerializeField, Tooltip("�Ռ��g�H���������1�t���[���̉�]�i540�̖񐔂œ��́j")]
    private int _smashRotationValue = 30;

    /// <summary>
    /// �Ռ��g�H��������̃W�����v
    /// </summary>
    private float _smashJumpValue;

    [SerializeField,Tooltip("�{�X����������v���C���[X")]
    private float _eyesight = 527;

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

    [SerializeField, Tooltip("�e�v���n�u")]
    private Shadow _shadowP;

    
    private Clear _cClear;

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
    private Coroutine _currentCoroutine,_attackCor;

    /// <summary>
    /// �d��
    /// </summary>
    private float _gravityScale = 0;

    [SerializeField, Tooltip("�d�͍ő�l�A�d�͉����x")]
    private float _gravityMax = 0.02f, _gravityDelta = 0.01f;

    [SerializeField]
    private AudioClip _acDeath, _acDamage, _acBlocked,_acClow, _acPress,_acBabbleCharge,_acBabbleFire,_acSmashed, _acWakeUp,_acJamp;


    // Start is called before the first frame update
    void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");

        _enemyCoreScript = GetComponent<EnemyCoreC>();

        //GetComponent<GraphicC>().ResetAnimation(0);
        //���G���[�h�I��
        _enemyCoreScript._isGetBlowAble = false;
        //�����ʒu�o�^
        _positionOwnFirst = transform.localPosition;

        _cClear = GameObject.Find("ClearManager").GetComponent<Clear>();
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
        _boss1ToWallRayHitR = Physics2D.Raycast(_boss1Ray.origin + transform.right + (-transform.up * transform.localScale.y * 0.3f), _boss1Ray.direction, 10, 8);
        _boss1ToWallRayHitL = Physics2D.Raycast(_boss1Ray.origin + transform.right + (-transform.up * transform.localScale.y * 0.3f), _boss1Ray.direction, 10, 8);

        //�������蔻��i������Ȃ��Ȃ���������Ԃ��j
        _boss1ToFloorRayHit = Physics2D.Raycast(_boss1Ray.origin - (transform.up * transform.localScale.y*5.0f), _boss1Ray.direction, 10, 8);

        //�������ߑ΍�
        if (_boss1ToWallRayHitR && _boss1ToWallRayHitL && _boss1ToFloorRayHit&&_boss1Mode!=6)
        {
            transform.position += transform.up * 0.3f;
        }
        //���܂ŗ�����
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
            //�d��
            if (_gravityScale < _gravityMax) _gravityScale += _gravityDelta;
            else _gravityScale = _gravityMax;
            Debug.Log("�R");
            TurnBack();
        }

        //�ʏ펞
        if (_boss1Mode == 0)
        {
            
            //����
            if (_positionPlayerNow.x>= _eyesight)
            {
                Debug.Log("����");
                _spriteYadokari.flipX = false;
                _boss1Mode = 1;
            }
        }
        //������
        if (_boss1Mode == 1)
        {
            if (_boss1Attack == 0)
            {
                //������
                if (_positionOwnNow.x - _eyeSight / 2 > _positionPlayerNow.x
                    || _gameobjectPlayer.transform.position.x > _positionOwnNow.x + _eyeSight / 2)
                {
                    Debug.Log("������");
                    //GetComponent<GraphicC>().ResetAnimation(0);
                    LookSet();
                    _boss1Mode = 0;
                }

                //�U���p��
                else
                {
                    //HP351�ȏ�
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
                    //HP50�ȉ�
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
                    Debug.Log("�{�X�R�[�h�F" + _boss1Attack);
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


        //�ӂ��Ƃё��]
        if (_boss1Mode == 3)
        {
            GetComponent<GraphicC>().ResetAnimation(11);
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
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            _spriteYadokari.flipY = false;
            _spriteYadokari.flipX = true;
            //�����Ă���
            transform.position += new Vector3(0.1f, -0.07f, 0);
        }

    }

    /// <summary>
    /// �c���U��
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
            Debug.Log("���h�J�����������I");
            _boss1ToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().PlayerDamage(1);
        }

        yield return new WaitForSeconds(0.02f*20);
        
        GetComponent<GraphicC>().ResetAnimation(0);
        _boss1Attack = 0;
    }

    /// <summary>
    /// �A�U��
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
    /// �W�����v�U��
    /// </summary>
    /// <returns></returns>
    private IEnumerator Jump()
    {
        _spriteYadokari.flipX = false;
        GetComponent<GraphicC>().ResetAnimation(7);
        GetComponent<AudioSource>().PlayOneShot(_acJamp);
        Vector3 _positionOwnShadow = transform.position - (transform.up * 6.6f);
        //�W�����v
        for(int i = 0; i < 100; i++)
        {
            transform.position += transform.up * 1.5f;
            yield return new WaitForSeconds(0.02f);
        }

        //�����킹
        for (int i = 0; i < 10; i++)
        {
            Chase(0, 5.0f);
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<GraphicC>().ResetAnimation(2);

        _positionOwnShadow.x = transform.position.x;
        Shadow _shadowInst= Instantiate(_shadowP, _positionOwnShadow, transform.localRotation);
        _shadowInst.Summoned(1.5f);
        //�~��
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
    /// �v���C���[�ǂ������s��
    /// </summary>
    private void Chase(float deray, float speed)
    {
        //�v���C���[����������
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


        //���WU�^�[��
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
    /// �Ռ��g�H����ĂЂ����肩����
    /// </summary>
    public void Smashed()
    {
        if (_boss1Mode == 1&&_bossCount!=3)
        {
            _sonicHp--;
        }
        if (_sonicHp <= 0)
        {
            //�ʏ킩�ړ����Ȃ�Ռ��g���ʂ���
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
    /// ���˂��h����A�N�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownAction()
    {
        yield return new WaitForSeconds(9.0f);
        GetComponent<AudioSource>().PlayOneShot(_acWakeUp);
        _boss1Mode = 5;
    }

    /// <summary>
    /// ��_���[�W
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
    /// ���A�N�V�����X�^�[�g
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
