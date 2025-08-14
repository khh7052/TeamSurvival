using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoolObj1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetActiveFalse", 2);
    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
