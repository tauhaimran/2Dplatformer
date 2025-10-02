using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatesaw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
                //rotate anticlockwise 2d saw
        transform.Rotate(0f, 0f, 0.5f);
        
    }
}
