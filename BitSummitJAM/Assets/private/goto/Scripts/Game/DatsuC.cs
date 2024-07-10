using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatsuC : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�v���C���[���")]
    private GameObject _gameobjectPlayer;

    /// <summary>
    /// �_�c�ʏ�s�����[�h
    /// -3=��
    /// -2=�Ђ��
    /// -1=�C��
    /// 0=�ʏ펞
    /// 1=�U������
    /// 2=�ːi��
    /// 3=�Ǔ˂��h����
    /// 4=�����ʒu�ɖ߂�
    /// </summary>
    private int _datsuMode;

    [SerializeField]
    [Tooltip("���񃂁[�h���̎��񑬓x�{��")]

    private float _moveSpeed;

    [SerializeField]
    [Tooltip("���񃂁[�h���̒��a�{��")]
    private float _moveMaxX;

    /// <summary>
    /// ���񃂁[�h����Sin����ϐ�
    /// </summary>
    private float _shukaiX;

    [SerializeField]
    [Tooltip("���G�͈͑傫���i���vpixel�j")]
    private float _eyeSight;

    [SerializeField]
    [Tooltip("�`���[�W�������x")]
    private float _chargeSpeed = 1.0f;

    [SerializeField, Tooltip("�`���[�W����")]
    private float _chargeTime = 3.0f;

    /// <summary>
    /// ���x�����l
    /// </summary>
    private float _speedDelta;

    /// <summary>
    /// �ːi���̕����w��
    /// </summary>
    private float _attackAngle;


    [SerializeField]
    [Tooltip("�ːi���x�{��")]
    private float _attackSpeed;

    /// <summary>
    /// �ǂ���o�Ă��珉�߂̃|�W�V�����ɖ߂�ۂ̑��x;
    /// </summary>
    private Vector3 _backSpeedDelta;

    /// <summary>
    /// �����̍��W
    /// </summary>
    private Vector3 _positionOwnNow;

    /// <summary>
    /// �����ʒu
    /// </summary>
    private Vector3 _positionOwnFirst;

    /// <summary>
    /// �ːi��Player�̍��W
    /// </summary>
    private Vector3 _positionPlayerTarget;

    /// <summary>
    /// �_�c���C�L���X�g��`
    /// </summary>
    private Ray _datsuRay;

    /// <summary>
    /// �_�cto�����C�L���X�g�Փ˒�`
    /// </summary>
    private RaycastHit2D _datsuToFloorRayHit;

    /// <summary>
    /// �_�ctoPlayre���C�L���X�g�Փ˒�`
    /// </summary>
    private RaycastHit2D _datsuToPlayerRayHit;

    /// <summary>
    /// 
    /// </summary>
    private Vector3 _lotationPlayerFirst;

    [SerializeField]
    [Tooltip("�_�c�X�v���C�g�ύX��")]
    private SpriteRenderer _spriteDatsu;

    /// <summary>
    /// �C�⒆
    /// </summary>
    private bool _isFainting;

    [SerializeField]
    [Tooltip("�C�⎞�������x�{��")]
    private float _faintingDropSpeed = 1;

    /// <summary>
    /// GraphicC�o�^
    /// </summary>
    private GraphicC _graphicScrpt;

    /// <summary>
    /// ECoreC�o�^
    /// </summary>
    private EnemyCoreC _enemyCoreScript;

    // Start is called before the first frame update
    void Start()
    {
        _gameobjectPlayer = GameObject.Find("PlayerManager");

        //��Ƀ_���[�W����炤
        GetComponent<EnemyCoreC>().IsGetDamage = true;

        _datsuMode = 0;
        //�����ʒu�o�^
        _positionOwnFirst = transform.localPosition;
        Debug.DrawRay(_positionOwnNow, new Vector3(0, 0, 1), Color.red, 10.0f, false);

        _lotationPlayerFirst=transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

        //���݈ʒu�X�V
        _positionOwnNow = transform.localPosition;

        //����BSin�ɗ^����ϐ��𑝉����Ă����B
        if(_moveMaxX!=0) _shukaiX += 1 / _moveMaxX;



        //360�x->0�x
        if (_shukaiX >= 360) _shukaiX = 0;

        //�ʏ펞
        if (_datsuMode == 0)
        {
            //�v���C���[�ʒu����
            _positionPlayerTarget = _gameobjectPlayer.transform.position;
            
            //�������BSin�ɕϐ������čs�����藈�����\������
            transform.position += new Vector3((Mathf.Sin(_shukaiX) * Mathf.Deg2Rad) * _moveSpeed, 0);
            //�܂�Ԃ���X���]����
            if (0 <= Mathf.Sin(_shukaiX) * Mathf.Deg2Rad) _spriteDatsu.flipX = true;
            else _spriteDatsu.flipX = false;

            //����
            if (_positionOwnNow.x - _eyeSight / 2 <= _positionPlayerTarget.x
                && _gameobjectPlayer.transform.position.x <= _positionOwnNow.x + _eyeSight / 2)
            {
                Debug.Log("����");
                //GetAngle��p���Č���������o��
                _attackAngle = GetAngle(_gameobjectPlayer.transform.position - _positionOwnNow);
                transform.localEulerAngles = new Vector3(0, 0, -_attackAngle+90);
                //�U���`���[�W�J�n
                StartCoroutine("AttackCharge");
                //���[�h�ύX
                _datsuMode = 1;
            }
        }

        //�G����
        else if (_datsuMode == 1)
        {
            _attackAngle = GetAngle(_gameobjectPlayer.transform.position - _positionOwnNow);
            //�����킹���Ă���V�X�e��
            transform.localEulerAngles = new Vector3(0, 0, -_attackAngle + 90);
            transform.position -= GetDirection(_attackAngle) * _chargeSpeed;
            
        }

        //�ˌ�
        else if (_datsuMode == 2)
        {
            transform.position += GetDirection(_attackAngle) * _attackSpeed;
            //�����o�^
            _datsuRay = new Ray(_positionOwnNow, new Vector3(0,0,0));
            //�����C���[��Ray���΂�
            _datsuToFloorRayHit = Physics2D.Raycast(_datsuRay.origin+transform.right*2.0f, _datsuRay.direction, 10, 8);
            if (_datsuToFloorRayHit.collider)
            {
                //�h����s���J�n
                StartCoroutine("Stinging");
                _datsuMode = 3;
            }
            //Player���C���[��Ray���΂�
            _datsuToPlayerRayHit = Physics2D.Raycast(_datsuRay.origin + transform.right * 2.0f, _datsuRay.direction, 10, 128);
            if (_datsuToPlayerRayHit.collider)
            {
                Debug.Log("�_�c���������I");
                Debug.Log(_datsuToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().name);
                Debug.Log(_datsuToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().Hp);
                _datsuToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().PlayerDamage(1);
                _datsuMode = 4;
                StartCoroutine("BackToFirstPosition");
            }
        }

        //�Ǔ˂��h���蒆
        else if (_datsuMode == 3)
        {
            transform.position -= GetDirection(_attackAngle) * _speedDelta;
        }

        //�����ʒu�ɖ߂�
        else if (_datsuMode == 4)
        {
            transform.position += _backSpeedDelta;
        }

        if (_datsuMode == -1)
        {
            //���ɂ�����܂ŗ�������
            _datsuRay = new Ray(_positionOwnNow, -transform.forward);
            //�����C���[��Ray���΂�
            _datsuToFloorRayHit = Physics2D.Raycast(_datsuRay.origin, _datsuRay.direction, 10,8);
            if (!_datsuToFloorRayHit.collider)
            {
                transform.position += -transform.up * _faintingDropSpeed;
            }
        }

        //���s��
        else if (_datsuMode == -3)
        {
            transform.localEulerAngles = Vector3.zero;
            _spriteDatsu.flipY = true;
            //_deathMoveSpeed -= _deathMoveDeltaY;
            //�Ђ�����Ԃ��ė����Ă䂭
            transform.position -= new Vector3(0, 0.07f, 0);
        }

        //�O���t�B�b�N���������܂ɂȂ�Ȃ��悤�ɒ��߁B��]��90~270�ɂȂ�����Y���]
        if (_datsuMode >= 0)
        {
            if (90 <= transform.localEulerAngles.z && transform.localEulerAngles.z < 270) _spriteDatsu.flipY = true;
            else _spriteDatsu.flipY = false;
        }
    }


    /// <summary>
    /// �͈͓���player���邩�ǂ����`�F�b�N
    /// </summary>
    /// <param name="positionOwn">�����̍��W</param>
    /// <param name="eyeSight">Player�̍��W</param>
    /// <param name="positionPlayer"></param>
    /// <returns>���܂����HTorF</returns>
    private bool InSphere(Vector3 positionOwn, float eyeSight, Vector3 positionPlayer)
    {
        var sum = 0f;
        for (var i = 0; i < 3; i++)
            sum += Mathf.Pow(positionOwn[i] - positionPlayer[i], 2);
        return sum <= Mathf.Pow(eyeSight, 2f);
    }

    /// <summary>
    /// 3�b�ԃ`���[�W(0.03*100)
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCharge()
    {
        GetComponent<GraphicC>().ResetAnimation(5);
        _spriteDatsu.flipX = true;
        yield return new WaitForSeconds(_chargeTime);
        GetComponent<GraphicC>().ResetAnimation(0);
        _datsuMode = 2;
    }

    /// <summary>
    /// ���A�N�V�����X�^�[�g
    /// </summary>
    public void OnDeath()
    {
        GetComponent<GraphicC>().ResetAnimation(1);
        StopAllCoroutines();
        _datsuMode = -3;
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// �����������s��
    /// </summary>
    /// <returns></returns>
    private IEnumerator Stinging()
    {
        //�˂��h����
        /*
        for (int i = 0; i < 10; i++)
        {
            transform.position += GetDirection(_attackAngle) * _attackSpeed;
            yield return new WaitForSeconds(0.03f);
        }
        */
        _speedDelta = 0;
        yield return new WaitForSeconds(0.5f);
        //�˂��h���������߂�
        _speedDelta = _attackSpeed;
        yield return new WaitForSeconds(0.09f);
        _speedDelta = 0;
        yield return new WaitForSeconds(0.5f);
        //�����𐳖ʂɖ߂�
        transform.localEulerAngles = new Vector3(0, 0, 0);
        //�ǂ���X�^�[�g�ʒu�ɖ߂�s���J�n
        StartCoroutine("BackToFirstPosition");
        _datsuMode = 4;
    }

    /// <summary>
    /// �ǂ���X�^�[�g�ʒu�ɖ߂�
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackToFirstPosition()
    {
        GetComponent<GraphicC>().ResetAnimation(0);
        //�����ʒu�ƌ��ݒn����߂�����ƃX�s�[�h������o��
        _backSpeedDelta = (_positionOwnFirst - _positionOwnNow) / 50;
        transform.localEulerAngles += new Vector3(0, 0, 180);
        //1.5�b�����Ė߂�
        yield return new WaitForSeconds(1.5f);
        _shukaiX = 0;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        //�ʏ펞�ɖ߂�
        _datsuMode = 0;
    }

    /// <summary>
    /// �C�┭��
    /// </summary>
    public void BeFainting()
    {
        
        if (_datsuMode!=-1)
        //_isFainting = true;
        StartCoroutine("DoFainting");
    }

    /// <summary>
    /// �C��A�N�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoFainting()
    {
        GetComponent<GraphicC>().ResetAnimation(11);
        Debug.Log("�C��");
        //���ׂẴR���[�`�����~�߂�
        StopCoroutine("AttackCharge");
        StopCoroutine("Stinging");
        StopCoroutine("BackToFirstPosition");
        _datsuMode = -1;
        transform.localEulerAngles = _lotationPlayerFirst;
        _spriteDatsu.flipY = true;

        //���܂ŗ�����
        yield return new WaitForSeconds(10.0f);
        //�ǂ���X�^�[�g�ʒu�ɖ߂�s���J�n
        StartCoroutine("BackToFirstPosition");
        _datsuMode = 4;
        Debug.Log("�C�╜�A");
    }


    /// <summary>
    /// ��_���[�W
    /// </summary>
    public void GetDamage()
    {

    }

    /// <summary>
    /// own����target�֐i�ލۂ̉��Z�l��Vector3�Ŋ���o��
    /// </summary>
    /// <param name="own">�i�܂������I�u�W�F�N�g�̍��W</param>
    /// <param name="target">�ړI���W</param>
    /// <returns>���Z�l�B�����own�ɑ�����target�Ɍ������Đi��</returns>
    private Vector3 GetMoveTarget(Vector3 own, Vector3 target)
    {
        Vector3 direction = target - own;
        float angleForTarget = GetAngle(direction);
        return GetDirection(angleForTarget);
    }

    /// <summary>
    /// A���猩��B�̌��������
    /// </summary>
    /// <param name="direction">Vector3 B-A</param>
    /// <returns>A���猩��B�̌���</returns>
    private float GetAngle(Vector3 direction)
    {
        float rad = Mathf.Atan2(direction.x, direction.y);
        return rad * Mathf.Rad2Deg;
    }

    /// <summary>
    /// �i�݂�������������Vector3�̉��Z�l������o��
    /// </summary>
    /// <param name="angle">�i�݂�������</param>
    /// <returns>���Z�l�B����𑫂���angle�Ɍ������Đi��</returns>
    private Vector3 GetDirection(float angle)
    {
        Vector3 direction = new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
        return direction;
    }

}
