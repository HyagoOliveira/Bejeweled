using UnityEngine;
using ActionCode.SceneManagement;

namespace Bejeweled.UI
{
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField, Tooltip("The game Scene."), Scene]
        private string gameScene = "Game";
        [SerializeField, Tooltip("The Scene Loading Settings asset.")]
        private SceneLoadingSettings loadingSettings;

        public void GoToGameScene() => SceneManager.LoadScene(gameScene, loadingSettings);
    }
}
