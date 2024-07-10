using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBreak : MonoBehaviour
{
    private float destroyTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Break(int hoge)
    {
        Debug.Log(hoge);

        if(hoge >= 80)
        {
            GetComponent<GraphicC>().ResetAnimation(1);
            Destroy(this.gameObject , destroyTime);
        }
    }
}
