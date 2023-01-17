using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public GameObject stamBar;
    public GameObject healthBar;

    private Slider StamBar;
    private Slider HealthBar;

    public Image stamFill;
    public Image healthFill;

    public GameObject COntrolsContainer;
    public GameObject Buttons;



    private void Awake()
    {
        StamBar = stamBar.GetComponent<Slider>();
        HealthBar = healthBar.GetComponent<Slider>();

       
    }
    private void FixedUpdate()
    {
        
        stamFill.color = new Color(0f, playerMovement.Stamina * 2f, 255f);

        StamBar.value = playerMovement.Stamina / 100f;
        HealthBar.value = playerMovement.Health / 100f;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            COntrolsContainer.SetActive(false);
        }

        if (playerMovement.IsDIe)
        {
            StamBar.enabled = false;
            HealthBar.enabled = false;

            
        }
        else
        {
            StamBar.enabled = true;
            HealthBar.enabled = true;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        
    }

    public void Controls()
    {
        COntrolsContainer.SetActive(true);
        Buttons.SetActive(false);
    }

    public void Exit()
    {
        COntrolsContainer.SetActive(false);
        Buttons.SetActive(true);
    }

}
