using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool local;    
	
	// Update is called once per frame
	void FixedUpdate () {
        if(local)
            transform.localPosition += Vector3.right * speed * Time.fixedDeltaTime;
        else
            transform.position += Vector3.right * speed * Time.fixedDeltaTime;

    }
}
