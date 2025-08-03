using UnityEngine;
using UnityEngine.SceneManagement;

namespace Busta.LoopRacers
{
    public class StartGame : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene(1);
        }
    }
}