using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for camera rotation around the egg in the starting scene
public class RotateAround : MonoBehaviour
{
    public Transform egg;
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(egg.position, Vector3.up, speed * Time.deltaTime);
    }
}
