using System.Collections;
using System.Collections.Generic;
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
        pos = new Vector2(ivyPos.x - transform.localScale.x * 0.5f, ivyPos.y - transform.localScale.y * 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(pos, transform.right,transform.localScale.x);

        if (hit)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<EnemyCoreC>().GetBlow(80);

            }

            if (hit.collider.CompareTag("Ground"))
            {
                Debug.Log(hit.collider.transform.position);

                Destroy(gameObject,1.0f);
                GetComponent<GraphicC>().ResetAnimation(1);
                fall = false;
                Debug.Log("’n–Ê“–‚½‚Á‚½");
            }
        }
    }

    private void FallMove()
    {
        ivyPos -= transform.up * speed * Time.deltaTime;

        Destroy(ivy);

        transform.position = ivyPos;
    }
}
