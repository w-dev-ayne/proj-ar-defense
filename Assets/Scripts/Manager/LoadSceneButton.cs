using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] 
    private string sceneName;

    private void OnEnable()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClickSceneButton);
    }

    private void OnClickSceneButton()
    {
        SceneManager.LoadScene(sceneName);
    }
}
