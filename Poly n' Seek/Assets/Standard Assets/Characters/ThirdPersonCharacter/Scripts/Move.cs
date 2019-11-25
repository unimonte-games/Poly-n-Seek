using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public float Intensidad;
    public float IntensidadJump;
    public float VelocidadeMax;
    // Use this for initialization
    private float timerJump;
    private bool timerJumpck;
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(gameObject.GetComponent<Rigidbody>().velocity.magnitude <= VelocidadeMax)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Intensidad * Time.deltaTime, ForceMode.Impulse);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (timerJumpck == false)
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.up * IntensidadJump * Time.deltaTime, ForceMode.Impulse);
                timerJumpck = true;
            }
        }
        if(timerJumpck == true)
        {
            timerJump += 1 * Time.deltaTime;
            if(timerJump >= 1f)
            {
                timerJump = 0;
                timerJumpck = false;
            }
        }
    }
}
            
       
    
