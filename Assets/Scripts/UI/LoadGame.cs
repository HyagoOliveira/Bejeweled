using UnityEngine;
using ActionCode.SceneManagement;

namespace Bejeweled.UI
{
    /// <summary>
    /// Loads the games Scene.
    /// </summary>
    public sealed class LoadGame : MonoBehaviour
    {
        [SerializeField, Tooltip("The game Scene."), Scene]
        private string gameScene = "Game";
        [SerializeField, Tooltip("The Scene Loading Settings asset.")]
        private SceneLoadingSettings settings;

        /// <summary>
        /// Loads the game scene fading the screen.
        /// </summary>
        public void GoToGameScene() => SceneManager.LoadScene(gameScene, settings);
    }
}
