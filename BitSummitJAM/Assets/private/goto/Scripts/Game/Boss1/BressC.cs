using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BressC : MonoBehaviour
{
    private Vector3 velocity, pos;
    private float sspeed, kkaso, aang;

    /// <summary>
    /// カニレイキャスト定義
    /// </summary>
    private Ray _bressRay;

    /// <summary>
    /// カニtoプレイヤー衝突判定
    /// </summary>
    private RaycastHit2D _bressToPlayerRayHit;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,5.0f);
    }

    public void EShot1(float angle, float speed, float kasoku)
    {
        var direction = GetDirection(angle);
        velocity = direction * speed;
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;

        sspeed = speed;
        kkaso = kasoku;
        aang = angle;
    }

    public Vector3 GetDirection(float angle)
    {
        Vector3 direction = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0);
        return direction;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = transform.position;

        _bressRay = new Ray(pos, new Vector3(0, 0, Mathf.Infinity));
        transform.localPosition += velocity;
        sspeed += kkaso;
        var direction = GetDirection(aang);
        velocity = direction * sspeed;

        //プレイヤーあたり判定（当たったら攻撃）
        _bressToPlayerRayHit = Physics2D.Raycast(_bressRay.origin, _bressRay.direction, 10, 128);
        if (_bressToPlayerRayHit)
        {
            _bressToPlayerRayHit.collider.gameObject.GetComponentInParent<PlayerManager>().PlayerDamage(1);
            Destroy(gameObject);
        }
    }
}
