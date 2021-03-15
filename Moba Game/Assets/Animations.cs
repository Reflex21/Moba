using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{

	public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator> ();
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKey(KeyCode.Q)){
			anim.SetTrigger("Q");
		}  
		if(Input.GetKey(KeyCode.W)){
			anim.SetTrigger("W");			
		}
		if(Input.GetKey(KeyCode.E)){
			anim.SetTrigger("E");			
		}    
    }
}
