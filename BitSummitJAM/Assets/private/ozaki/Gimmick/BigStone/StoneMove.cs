using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMove : MonoBehaviour
{
    public bool Hit;

    // Start is called before the first frame update
    void Start()
    {
        Hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Mover(int hoge,bool fuga)
    {
        Hit = true;

        Debug.Log("‚Å‚©‚¢ŠâˆÚ“®");
    }
}
