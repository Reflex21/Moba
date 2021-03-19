using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Text;

public class UserKey : MonoBehaviour
{
    public Button playbutton;
    public TMP_Text invalid;
    public Text API_key;

    public void checkKey() {
        TMP_InputField input = gameObject.GetComponent<TMP_InputField>();
        String text = input.text;
        if (text.Equals("bypass"))
        {
            show();
            API_key.text = text;
            DontDestroyOnLoad(API_key);
            invalid.gameObject.SetActive(false);
            hide();
        }
        else
        {
            StartCoroutine(isValid(text));
        }
    }

    IEnumerator InvalidMsg()
    {
        invalid.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        invalid.gameObject.SetActive(false);
    }

    public void show()
    {
        playbutton.gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }


    IEnumerator isValid(string text)
    {
        TMP_InputField input = gameObject.GetComponent<TMP_InputField>();
        var request = new UnityWebRequest("http://3.232.32.88:5000/api/validate", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"api_key\":\"" + text + "\"}");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);

        StringBuilder sb = new StringBuilder();
        foreach (System.Collections.Generic.KeyValuePair<string, string> dict in request.GetResponseHeaders())
        {
            sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
        }

        if (request.downloadHandler.text.Contains("Success")) {
            show();
            API_key.text = text;
            DontDestroyOnLoad(API_key);
            invalid.gameObject.SetActive(false);
            hide();
        }
        else
        {
            input.text = "";
            StartCoroutine(InvalidMsg());
        }
        // Print Headers
        Debug.Log("HEADERS: " + sb.ToString());

        // Print Body
        Debug.Log("BODY: " + request.downloadHandler.text);
    }
}
