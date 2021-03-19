using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupAPI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text api = GameObject.FindWithTag("API_Key").GetComponent<Text>();
        gameObject.transform.GetComponent<Text>().text = api.text;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
