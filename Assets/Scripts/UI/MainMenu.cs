using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bejeweled.UI
{
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField, Tooltip("The game Scene name.")]
        private string gameSceneName = "Game";

        public void GoToGameScene() => SceneManager.LoadScene(gameSceneName);
    }
}