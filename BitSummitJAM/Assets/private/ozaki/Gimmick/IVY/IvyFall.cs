using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class IvyFall : MonoBehaviour
{
    bool fall;

    Vector3 pos;

    [SerializeField]
    float speed = 0.5f;

    Vector3 ivyPos;

    [SerializeField]
    GameObject ivy;

    // Start is called before the first frame update
    void Start()
    {
        fall = false;

        ivyPos = transform.position;

        pos = new(ivyPos.x - transform.localScale.x * 0.5f, ivyPos.y - transform.localScale.y * 0.5f);

        Rigidbody rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fall)
        {
            FallMove();

            HitCheck();
        }

    }
    

    public void Fall()
    {
        fall = true;
    }

    private void HitCheck()
    {
        pos = new Vector2(ivyPos.x - transform.localScale.x * 0.7f, ivyPos.y - transform.localScale.y * 0.5f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, transform.right,transform.localScale.x * 2.5f);

        Debug.DrawRay(pos, transform.right * transform.localScale.x * 2.5f, Color.red);


        foreach (var hit in hits)
        {
            if (hit)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<EnemyCoreC>().GetScrap();

                }

                if (hit.collider.CompareTag("Ground"))
                {
                    Debug.Log(hit.collider.transform.position);

                    Destroy(gameObject, 1.0f);

                    GetComponent<GraphicC>().ResetAnimation(1);

                    fall = false;

                    Debug.Log("’n–Ê“–‚½‚Á‚½");
                }
            }
        }
    }

    private void FallMove()
    {
        ivyPos -= transform.up * speed * Time.deltaTime;

        Destroy(ivy);

        transform.position = ivyPos;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision)
    //    {
    //        if (collision.gameObject.CompareTag("Enemy"))
    //        {
    //            collision.gameObject.GetComponent<EnemyCoreC>().GetScrap();
    //        }
    //        if (collision.CompareTag("Ground"))
    //        {
    //            Destroy(gameObject, 1.0f);

    //            GetComponent<GraphicC>().ResetAnimation(1);

    //            fall = false;

    //            Debug.Log("’n–Ê“–‚½‚Á‚½");
    //        }
    //    }
    //}
}
