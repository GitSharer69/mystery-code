using UnityEngine;
using UnityEngine.SceneManagement;

public class Handler : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

}
