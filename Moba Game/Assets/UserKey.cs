using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UserKey : MonoBehaviour
{
    public Button playbutton;
    public TMP_Text invalid;

    public void checkKey() {
        TMP_InputField input = gameObject.GetComponent<TMP_InputField>();
        String text = input.text;
        if (isValid(text))
        {
            show();
            invalid.gameObject.SetActive(false);
            hide();
        }
        else {
            input.text = "";
            StartCoroutine(InvalidMsg());
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

    public bool isValid(String t) {
        if (t.Length > 5) {
            return true;
        }
        return false;
    }
}
