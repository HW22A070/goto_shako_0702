using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public void Summoned(float deletetime)
    {
        transform.localScale = new Vector3(0, 1, 1);
        StartCoroutine(ShadowBigger(deletetime));
        Destroy(gameObject,deletetime);
    }

    private IEnumerator ShadowBigger(float delete)
    {
        float count = delete*50;
        for (int i = 0; i < count; i++)
        {
            transform.localScale = new Vector3(i/count,1,1);
            yield return new WaitForSeconds(0.02f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
