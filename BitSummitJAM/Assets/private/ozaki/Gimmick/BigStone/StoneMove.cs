using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoneMove : MonoBehaviour
{
    int Attack;

    private bool isRight;

    private bool hit;

    [SerializeField]
    [Tooltip("ˆÚ“®‘¬“x")]
    private float moveSpeed;

    [SerializeField]
    [Tooltip("—Ž‰º‘¬“x")]
    private float fallSpeed;

    private Vector3 stonePos;

    private float nowSpeed;

    [SerializeField]
    private LayerMask mask;

    private Ray ray;

    private bool isGround;
    // Start is called before the first frame update
    void Start()
    {
        stonePos = transform.position;

        nowSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(hit)
        {
            HitCheck(isRight);
        }
    }

    private void FixedUpdate()
    {
        if (hit)
        {
            if(isRight)
            {
                stonePos = stonePos + transform.right * nowSpeed;
            }
            else
            {
                stonePos = stonePos - transform.right * nowSpeed;
            }

            transform.position = stonePos;
        }
    }

    public void Mover(int hoge,bool fuga)
    {
        hit = true;

        isRight = fuga;

        Attack = hoge;

        Debug.Log("‚Å‚©‚¢ŠâˆÚ“®");

        nowSpeed = moveSpeed;
    }

    private void HitCheck(bool hoge)
    {
        if(hoge)
        {
            var rightpos = new Vector2(transform.position.x + transform.localScale.x * 0.5f,
                                  transform.position.y - transform.localScale.y * 0.5f);

            var hit = Physics2D.Raycast(rightpos,
                                        transform.up,
                                        transform.localScale.x * 0.9f,
                                        mask);

            Debug.DrawRay(rightpos, transform.up,Color.red, transform.localScale.x);

            if(hit)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log(hit.collider);

                    hit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(Attack);
                }
                else if(hit.collider.CompareTag("Ground"))
                {
                    Debug.Log(hit.collider);

                    stonePos = transform.position;

                    Destroy(this.gameObject);

                    Reset();
                }
            }
        }
        else
        {
            var leftpos = new Vector2 (transform.position.x - transform.localScale.x,
                          transform.position.y - transform.localScale.y);


            var hit = Physics2D.Raycast(leftpos,
                                        transform.up,
                                        transform.localScale.x * 0.9f,
                                        mask);
            if (hit)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(Attack);
                }
                else if (hit.collider.CompareTag("Ground"))
                {
                    Destroy(this.gameObject);

                    Reset();
                }
            }
        }
    }

    private void Reset()
    {
        Attack = 0;

        isRight = false;

        hit = false;

        nowSpeed = 0;
    }

    private void FallCheck()
    {
        if(Physics2D.Raycast(transform.position,
                             -transform.up,
                             transform.localScale.y * 0.5f,
                             mask))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }
}
