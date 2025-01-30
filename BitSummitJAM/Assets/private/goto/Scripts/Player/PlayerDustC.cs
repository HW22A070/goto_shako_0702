using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDustC : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    // Update is called once per frame
    public void Hanten(bool isRight)
    {
        if (isRight) transform.localEulerAngles = new Vector3(0f, 180f, 0f);
    }
}
