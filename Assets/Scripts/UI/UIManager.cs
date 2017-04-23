/************************************
** Created by Wizcas (wizcas.me)
************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager> 
{
    public Canvas canvas;

    public GameObject intro;
    public GameObject instructions;

    public GUISkin tooltipSkin;

    void Awake()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        ToIntro();
    }

    private void HideAll()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void ToIntro()
    {
        if (intro == null) return;
        HideAll();
        intro.gameObject.SetActive(true);
    }

    public void ToInstructions()
    {
        if (instructions == null) return;
        HideAll();
        instructions.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        HideAll();
        SceneManager.LoadScene(1);
    }
}
