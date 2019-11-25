using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColCheck : MonoBehaviour {

    void Start()
    {
        Destroy(gameObject, 10);
    }
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Prop")
        {
            if (other.GetComponent<CapturePlayerSync>().PlayerScript.GetComponent<PlayerSync>().IsHunterSync == false)
            {
                other.GetComponent<CapturePlayerSync>().PlayerScript.GetComponent<PlayerSync>().Morreu = true;
            }
        }
        else
        {
            Destroy(gameObject);
            Debug.Log(other.name + " Que colidiu");
        }
    }
}
