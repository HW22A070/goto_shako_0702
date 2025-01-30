using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�Ռ��g�̑���")]
    private float shockSpeed;

    private Vector3 moveDirection;

    private float movetime;

    /// <summary>
    /// �Ռ��g���C�L���X�g��`
    /// </summary>
    private Ray _shockRay;

    /// <summary>
    /// �Ռ��gto�G���C�L���X�g�Փ˒�`
    /// </summary>
    private RaycastHit2D _shockToEnemyRayHit;

    [SerializeField, Tooltip("�_���[�W")]
    private GameObject _damageEffect;

    public Vector3 MoveDirection
    {
        get => moveDirection;
        set => moveDirection = value;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        Move();

        ShockHit();
    }

    public void Move()
    {
        // ������ꂽ�ړ����v�Z
        transform.position += moveDirection * shockSpeed * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    private void ShockHit()
    {
        _shockRay = new Ray(transform.position, new Vector3(0, 0, Mathf.Infinity));
        _shockToEnemyRayHit = Physics2D.Raycast(_shockRay.origin, _shockRay.direction, 511);
        if (_shockToEnemyRayHit)
        {
            GameObject damageEffect = Instantiate(_damageEffect, _shockRay.origin + new Vector3(-1, 1, 0), transform.localRotation);
            Destroy(damageEffect, 0.2f);
            if (_shockToEnemyRayHit.collider.CompareTag("Enemy"))
            {
                _shockToEnemyRayHit.collider.gameObject.GetComponent<EnemyCoreC>().GetSonic();
                Destroy(gameObject);
            }

            if(_shockToEnemyRayHit.collider.CompareTag("Gimmick"))
            {
                _shockToEnemyRayHit.collider.gameObject.GetComponent<GimmickManager>().ShockGimmickHit();

                Destroy(gameObject);
            }
            if (_shockToEnemyRayHit.collider.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
            
        }
    }
}
