using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug=UnityEngine.Debug;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class Cylinder : MonoBehaviour
{
    public GameObject ring;
    public Renderer rend;
    public Vector3 pos;
    public Vector3 position;
    public float time;
	public string apikey;


	// called zero
	void Awake()
	{
		position = new Vector3();
		pos = new Vector3();
		time = Time.time;
		apikey = GameObject.FindWithTag("API_Key").GetComponent<Text>().text;
	}


	void OnMouseDown(){
		Animator anim = GameObject.Find("Jammo").GetComponent<Animator> ();
		if (!anim.GetBool("MidActivity") && !GetComponent<Renderer>().material.color.Equals(new Color(0.2f, .8f, .2f, .55f))){
			time = Time.time - (time + Time.deltaTime);
			if (!apikey.Equals("bypass")) StartCoroutine(sendData((time*1000).ToString()));
			reflex("ReFlex: " + time + "sec\nKey: MouseClick");
			TMP_Text text = GameObject.FindWithTag("MainText").GetComponent<TMP_Text>();
			text.text = "Nice!";
			rend = GetComponent<Renderer>();
			pos = gameObject.transform.position;
			position = new Vector3();
			rend.material.SetColor("_Color", new Color(0.2f, .8f, .2f, .55f));
		}
	}

	IEnumerator Wait(){
		Animator anim = GameObject.Find("Jammo").GetComponent<Animator> ();
		anim.SetBool("MidActivity", true);
		TMP_Text text = GameObject.FindWithTag("MainText").GetComponent<TMP_Text>();
		string[] keys = new string[] {"Q", "W", "E", "R", "1", "2", "3"};
		int rand = Random.Range(2, 7);
		for (int j = 0; j < rand; j++){
			int i = 0;
			string x = keys[Random.Range(0, 7)];
	    	text.text = "Press \"" + x + "\"";
	    	time = Time.time;
	    	while(!Input.GetKeyDown(x.ToLower())){
	    		if(Input.anyKeyDown){
	    			i++;
	    		}
	    		yield return null;
	    	}
	    	time = Time.time - (time + Time.deltaTime);
			if (!apikey.Equals("bypass")) StartCoroutine(sendData((time*1000).ToString()));
			reflex("ReFlex: " + time + "sec\nKey: " + x + ", Attempts: " + (i+1));
			text.text = "";
			yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
    	}
		text.text = "Well Done!";
		yield return new WaitForSeconds(.5f);
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		rend.material.SetColor("_Color", new Color(0.878f, .0784f, .6509f, 1.0f));
		text.text = "Click the Pink Target!";
		anim.SetBool("MidActivity", false);
		time =  Time.time;
	}

	public void reflex(string s){
        TMP_Text text = GameObject.FindWithTag("Feedback").GetComponent<TMP_Text>();
        string str = "--------------------------------\n" + s + "\n" + text.text;
		text.text = str;
        //string[] ss = str.Split('\n');
        //if (ss.Length > 10)
        //{
        //    text.text = ss[0] + "\n" + ss[1] + "\n\n" +
        //    ss[3] + "\n" + ss[4] + "\n\n" +
        //    ss[6] + "\n" + ss[7];
        //}
        //else
        //{
        //    text.text = str;
        //}

    }

	public void OnTriggerEnter(Collider other)
    {
		for (int i = 0; i < 10; i++){
			position = new Vector3(Random.Range(-7.0F, 7.0F), 0.2F, Random.Range(-7.0F, 6.0F));
			if (Vector3.Distance(pos, position) > 6f){
				break;
			}
		}
		transform.position = position;
		rend = GetComponent<Renderer>();
		rend.enabled = false;
    }

    public void OnTriggerExit(Collider collisionInfo){
		TMP_Text text = GameObject.FindWithTag("MainText").GetComponent<TMP_Text>();
    	StartCoroutine(Wait());
    }

	IEnumerator sendData(string data)
	{
		TMP_InputField input = gameObject.GetComponent<TMP_InputField>();
		var request = new UnityWebRequest("http://3.232.32.88:5000/api/data/unity", "POST");

        string json = "{\"api_key\":\"" + apikey + "\","
			+ "\"data\":[{\"type\":\"reaction\", \"game\":\"moba\", \"datapoints\":["+data+"]}]}";

		byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		Debug.Log("Status Code: " + request.responseCode);
	}
}
