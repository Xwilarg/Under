using UnityEngine;
using UnityEngine.SceneManagement;

namespace VarVarGamejam.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void LoadGame()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
