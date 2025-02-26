using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SceneLoader : MonoBehaviour
{
    public GameObject rulesMenu;
    public GameObject levelMenu;
    public GameObject homeMenu;
    const int HOME = 0;
    const int LEVEL_1 = 1;
    const int LEVEL_2 = 2;
    

    void Start(){
        rulesMenu.SetActive(false);
        levelMenu.SetActive(false);
        homeMenu.SetActive(true); // start up on home screen

    }


    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void RulesLogicInHome()
    {
        homeMenu.SetActive(false);
        rulesMenu.SetActive(true);
        
    }

    public void ReturnToHome(){
        rulesMenu.SetActive(false);
        homeMenu.SetActive(true);
    }

        public void loadScene(int scene)
    {
        if (scene == LEVEL_1) SceneManager.LoadSceneAsync(LEVEL_1); // load the first level
        if (scene == LEVEL_2) SceneManager.LoadSceneAsync(LEVEL_2); // load the second level
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

    public void LevelSelection()
    {
        homeMenu.SetActive(false);
        levelMenu.SetActive(true);
    }

    

    
}

