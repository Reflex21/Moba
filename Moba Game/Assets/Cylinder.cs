using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug=UnityEngine.Debug;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using UnityEngine.AI;

public class Cylinder : MonoBehaviour
{
    public GameObject ring;
    public Renderer rend;
    public Vector3 pos;
    public Vector3 position;
    public float time;
	public string apikey;
	public int mouse;
	public List<reaction> reactions;
	public int round;
	public Button roundButton;

	public class reaction {
		string key;
		float accuracy;
		float react;

		public reaction(string k, float a, float r) {
			key = k;
			accuracy = a;
			react = r; 
		}
		public string getKey() {
			return key;
		}
		public float getAccuracy()
		{
			return accuracy;
		}
		public float getTime()
		{
			return react;
		}

	}

    private void Awake()
    {
		Animator anim = GameObject.Find("Jammo").GetComponent<Animator>();
		anim.SetBool("MidActivity", true);
	}

    // called zero
    public void StartRound()
	{
		roundButton.gameObject.SetActive(false);
		Animator anim = GameObject.Find("Jammo").GetComponent<Animator>();
		anim.SetBool("MidActivity", false);
		reactions = new List<reaction>();
		position = new Vector3();
		pos = new Vector3();
		time = Time.time;
		try
		{
			apikey = GameObject.FindWithTag("API_Key").GetComponent<Text>().text;
		}
		catch {
			apikey = "bypass";
		}
		mouse = Random.Range(1, 5);
		round = 12;
	}

	bool endRound() {
		round = round - 1;
		Debug.Log("END ROUND: " + round.ToString());
		if (round <= 0)
		{
			GameObject.Find("Jammo").GetComponent<NavMeshAgent>().destination = new Vector3(0, 0, 0);
			Animator anim = GameObject.Find("Jammo").GetComponent<Animator>();
			anim.SetBool("MidActivity", true);
			roundButton.gameObject.SetActive(true);
			TMP_Text roundText = GameObject.Find("RoundText").GetComponent<TMP_Text>();
			string key = "";
			float avgMR = 0;
			float avgKR = 0;
			float avgMA = 0;
			float avgKA = 0;
			int ms = 0;
			int ks = 0;
			reaction[] rs = reactions.ToArray();
			for (int i = 0; i < rs.Length; i++) {
				key = rs[i].getKey();
				if (key.Equals("Mouse")) {
					ms++;
					avgMR += rs[i].getTime();
					avgMA += rs[i].getAccuracy();
				}
                else {
					ks++;
					avgKR += rs[i].getTime();
					avgKA += rs[i].getAccuracy();
				}
			}
			roundText.fontSize = 64;
			roundText.text =
				"\t\tRound Summary\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"+
				"\nAvg Mouse Reaction:\t" + (avgMR/ms).ToString() +
				"\nAvg Key Reaction:\t\t" + (avgKR / ks).ToString() +
				"\nAvg Mouse Accuracy:\t" + ((ms / avgMA)*100).ToString() + "%" +
                "\nAvg Key Accuracy:\t\t" + ((ks / avgKA)*100).ToString() + "%" +
				"\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
				"Click here to start new round";
			if (!apikey.Equals("bypass")) StartCoroutine(sendData(reactions, (ms / avgMA).ToString(), (ks / avgKA).ToString()));
			reactions = new List<reaction>();
			return true;
		}
		else {
			return false;
		}
	}

	void OnMouseDown(){
		Animator anim = GameObject.Find("Jammo").GetComponent<Animator> ();
		if (!anim.GetBool("MidActivity") && !GetComponent<Renderer>().material.color.Equals(new Color(0.2f, .8f, .2f, .55f))){
			time = Time.time - (time + Time.deltaTime);
			reactions.Add(new reaction("Mouse", 1, time));
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
		string x = "";
		for (int j = 0; j < rand; j++)
		{
			x += keys[Random.Range(0, 7)] + "-";
		}
		x = x.Substring(0, x.Length - 1);
		int i = 0;
		text.text = "Press: \"" + x + "\"";
	    time = Time.time;
	    while(x.Length > 0){
	    	if(Input.anyKeyDown){
	    		i++;
	    	}
			if (Input.GetKeyDown(x.Substring(0, 1).ToLower())) {
				time = Time.time - (time + Time.deltaTime);
				reactions.Add(new reaction(x.Substring(0, 1), i, time));
				reflex("ReFlex: " + time + "sec\nKey: " + x.Substring(0, 1) + ", Attempts: " + (i));
				x = x.Substring(Mathf.Min(2, x.Length));
				text.text = "Press \"" + x + "\"";
				i = 0;
				time = Time.time;
			}
			yield return null;
	    }
		text.text = "";
		yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
		text.text = "Well Done!";
		yield return new WaitForSeconds(.5f);
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		rend.material.SetColor("_Color", new Color(0.878f, .0784f, .6509f, 1.0f));
		text.text = "Click the Pink Target!";
		anim.SetBool("MidActivity", false);
		time = Time.time;
		endRound();
	}

	public void reflex(string s){
        TMP_Text text = GameObject.FindWithTag("Feedback").GetComponent<TMP_Text>();
        string str = "--------------------------------\n" + s + "\n" + text.text;
        text.text = str;
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
		mouse = mouse - 1;
		if (mouse <= 0) {
			if (!endRound())
			{
				mouse = Random.Range(1, 5);
				StartCoroutine(Wait());
			}
			else {
				rend = GetComponent<Renderer>();
				rend.enabled = true;
				rend.material.SetColor("_Color", new Color(0.878f, .0784f, .6509f, 1.0f));
				time = Time.time;
			}
		}
		else
		{
			rend = GetComponent<Renderer>();
			rend.enabled = true;
			rend.material.SetColor("_Color", new Color(0.878f, .0784f, .6509f, 1.0f));
			time = Time.time;
		}
    }

	IEnumerator sendData(List<reaction> datas, string ma, string ka)
	{
		TMP_InputField input = gameObject.GetComponent<TMP_InputField>();
		var request = new UnityWebRequest("http://3.232.32.88:5000/api/data/unity", "POST");
		string mousepoints = "";
		string keypoints = "";
		string qpoints = "";
		string wpoints = "";
		string epoints = "";
		string rpoints = "";
		string onepoints = "";
		string twopoints = "";
		string threepoints = "";
		reaction[] rs = datas.ToArray();
		for (int i = 0; i < rs.Length; i++)
		{
			if (rs[i].getKey().Equals("Mouse")) {
				if (mousepoints.Equals("")) mousepoints += (rs[i].getTime() * 1000).ToString();
				else mousepoints += ", " + (rs[i].getTime() * 1000).ToString();
			}
			else {
				if (keypoints.Equals("")) keypoints += (rs[i].getTime() * 1000).ToString();
				else keypoints += ", " + (rs[i].getTime() * 1000).ToString();

				if (rs[i].getKey().Equals("Q")) {
					if (qpoints.Equals("")) qpoints += (rs[i].getTime() * 1000).ToString();
					else qpoints += ", " + (rs[i].getTime() * 1000).ToString();
				}
				else if (rs[i].getKey().Equals("W"))
				{
					if (wpoints.Equals("")) wpoints += (rs[i].getTime() * 1000).ToString();
					else wpoints += ", " + (rs[i].getTime() * 1000).ToString();
				}
				else if (rs[i].getKey().Equals("E"))
				{
					if (epoints.Equals("")) epoints += (rs[i].getTime() * 1000).ToString();
					else epoints += ", " + (rs[i].getTime() * 1000).ToString();
				}
				else if (rs[i].getKey().Equals("R"))
				{
					if (rpoints.Equals("")) rpoints += (rs[i].getTime() * 1000).ToString();
					else rpoints += ", " + (rs[i].getTime() * 1000).ToString();
				}
				else if (rs[i].getKey().Equals("1"))
				{
					if (onepoints.Equals("")) onepoints += (rs[i].getTime() * 1000).ToString();
					else onepoints += ", " + (rs[i].getTime() * 1000).ToString();
				}
				else if (rs[i].getKey().Equals("2"))
				{
					if (twopoints.Equals("")) twopoints += (rs[i].getTime() * 1000).ToString();
					else twopoints += ", " + (rs[i].getTime() * 1000).ToString();
				}
				else if (rs[i].getKey().Equals("3"))
				{
					if (threepoints.Equals("")) threepoints += (rs[i].getTime() * 1000).ToString();
					else threepoints += ", " + (rs[i].getTime() * 1000).ToString();
				}

			}
		}
			string json = "{\"api_key\":\"" + apikey + "\",\"data\":[" +

            "{\"type\":\"reaction_mouse\", \"game\":\"moba\", \"datapoints\":[" +
			mousepoints + "]}," +
			"{\"type\":\"reaction_key\", \"game\":\"moba\", \"datapoints\":[" +
			keypoints + "]}," +

			"{\"type\":\"reaction_Q\", \"game\":\"moba\", \"datapoints\":[" +
			qpoints + "]}," +
			"{\"type\":\"reaction_W\", \"game\":\"moba\", \"datapoints\":[" +
			wpoints + "]}," +
			"{\"type\":\"reaction_E\", \"game\":\"moba\", \"datapoints\":[" +
			epoints + "]}," +
			"{\"type\":\"reaction_R\", \"game\":\"moba\", \"datapoints\":[" +
			rpoints + "]}," +
			"{\"type\":\"reaction_1\", \"game\":\"moba\", \"datapoints\":[" +
			onepoints + "]}," +
			"{\"type\":\"reaction_2\", \"game\":\"moba\", \"datapoints\":[" +
			twopoints + "]}," +
			"{\"type\":\"reaction_3\", \"game\":\"moba\", \"datapoints\":[" +
			threepoints + "]}," +

			"{\"type\":\"accuracy_mouse\", \"game\":\"moba\", \"datapoints\":[" +
			ma + "]}," +
			"{\"type\":\"accuracy_key\", \"game\":\"moba\", \"datapoints\":[" +
			ka + "]}]}";

		byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		Debug.Log("Status Code: " + request.responseCode);
	}
}
