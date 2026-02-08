using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class mainmenucontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CanvasGroup OptionPanel;

    public void Start()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Option()
    {
        OptionPanel.alpha = 1;
        OptionPanel.blocksRaycasts = true;
    }

    public void Back()
    {
        OptionPanel.alpha = 0;
        OptionPanel.blocksRaycasts = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
