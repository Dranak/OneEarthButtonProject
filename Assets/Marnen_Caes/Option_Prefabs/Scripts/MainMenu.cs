using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
    public class MainMenu : MonoBehaviour
    {
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(0);
        }
        public void PreviousLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

