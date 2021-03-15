using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public void Play(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Home() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    public void Resume()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ApiKey() {

	}

    public void Quit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
