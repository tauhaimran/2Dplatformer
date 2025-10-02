using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (player != null)
    {
        Vector3 newPos = player.transform.position;
        newPos.z = transform.position.z; // Keep camera's Z position
        transform.position = newPos;
    }
    }
}
