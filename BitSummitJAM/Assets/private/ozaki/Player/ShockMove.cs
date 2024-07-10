using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("衝撃波の速さ")]
    private float shockSpeed;

    private Vector3 moveDirection;

    private float movetime;

    /// <summary>
    /// 衝撃波レイキャスト定義
    /// </summary>
    private Ray _shockRay;

    /// <summary>
    /// 衝撃波to敵レイキャスト衝突定義
    /// </summary>
    private RaycastHit2D _shockToEnemyRayHit;

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
        // 投げられた移動を計算
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

            if(_shockToEnemyRayHit.collider.CompareTag("Enemy"))
            {
                _shockToEnemyRayHit.collider.gameObject.GetComponent<EnemyCoreC>().GetSonic();
                Destroy(gameObject);
            }

            if(_shockToEnemyRayHit.collider.CompareTag("Gimmick"))
            {
                _shockToEnemyRayHit.collider.gameObject.GetComponent<GimmickManager>().GimmickHit();

                Destroy(gameObject);
            }
            if (_shockToEnemyRayHit.collider.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
            
        }
    }
}
