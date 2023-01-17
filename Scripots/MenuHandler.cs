using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuHandler : MonoBehaviour
{
    public BotInfo[] botInfo;
    public Renderer RenderingObject;

    public Transform directionalLight;

    public GameObject PauseScreenContainer;
    public GameObject NormScreenContainer;

    
    private void Awake()
    {
        RandomSprite();
        PauseScreenContainer.SetActive(false);
        NormScreenContainer.SetActive(true);
        int RandomNumber = Random.Range(0, 20);

        if (RandomNumber == 7)
        {
            Spooky();
        }
        else
        {
            directionalLight.localRotation = Quaternion.Euler(50,30,0);
            RenderingObject.enabled = false;
        }
    }
    private void Spooky()
    {
        RandomSprite();

        directionalLight.localRotation = Quaternion.Euler(-90, 0, 0);

        
        
    }
    private void RandomSprite()
    {
        
        int SpriteNumber = Random.Range(0, botInfo.Length);

        RenderingObject.sharedMaterial = botInfo[SpriteNumber].material;

    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Controls()
    {
        PauseScreenContainer.SetActive(true);
        NormScreenContainer.SetActive(false);
    }

    public void ExitPauseScrn()
    {
        PauseScreenContainer.SetActive(false);
        NormScreenContainer.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    

    [System.Serializable]
    public struct BotInfo
    {
        public string BotName;
        public Material material;
        
    }

}
