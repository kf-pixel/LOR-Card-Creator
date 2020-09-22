using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
	public IntVariable restarts;

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadSceneByName(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void LoadSceneByIndex(int i)
	{
		SceneManager.LoadScene(i);
	}

	public void NextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	private void Start()
	{
		if (restarts.value == 0)
		{
			restarts.value++;
			RestartScene();
		}
	}
}
