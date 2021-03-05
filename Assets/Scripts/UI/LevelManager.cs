using UnityEngine;
using UnityEngine.UI;
using Bejeweled.Macth;

namespace Bejeweled.UI
{
    /// <summary>
    /// Level Manager component.
    /// <para>You <see cref="NextLevel"/> and <see cref="PreviousLevel"/> function to switch between level.</para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField, Tooltip("The child Score Bar component.")]
        private SliderBar scoreBar;
        [SerializeField, Tooltip("The child Text component to show current level.")]
        private Text currentLevel;
        [SerializeField, Tooltip("The level settings assets.")]
        private BoardSettings[] levelSettings = new BoardSettings[0];

        /// <summary>
        /// The current MatchLevelSettings used to populate the board. 
        /// </summary>
        public BoardSettings CurrentLevelSettings
            => levelSettings[currentLevelSettingsIndex];

        /// <summary>
        /// Total level count.
        /// </summary>
        public int LevelsCount => levelSettings.Length;

        private Board board;
        private int currentLevelSettingsIndex = 0;

        private void Awake()
        {
            board = FindObjectOfType<Board>();
            UpdateVisualComponents();
        }

        private void OnEnable()
        {
            if (board) board.OnIncreaseScore += IncreaseScore;
        }

        private void OnDisable()
        {
            if (board) board.OnIncreaseScore -= IncreaseScore;
        }

        /// <summary>
        /// Go to the next level.
        /// </summary>
        public void NextLevel()
        {
            currentLevelSettingsIndex++;
            if (currentLevelSettingsIndex >= LevelsCount)
                currentLevelSettingsIndex = 0;

            if (board) board.Populate(CurrentLevelSettings);
            UpdateVisualComponents();
        }

        /// <summary>
        /// Go to the previous level.
        /// </summary>
        public void PreviousLevel()
        {
            currentLevelSettingsIndex--;
            if (currentLevelSettingsIndex < 0)
                currentLevelSettingsIndex = LevelsCount - 1;

            if (board) board.Populate(CurrentLevelSettings);
            UpdateVisualComponents();
        }

        private void IncreaseScore(int score)
        {
            scoreBar.CurrentValue += score;
            if (scoreBar.IsComplete()) OnScoreComplete();
        }

        private void OnScoreComplete()
        {
            scoreBar.UpdateInfoText("Level Completed!!");
        }

        private void UpdateVisualComponents()
        {
            UpdateScoreBar();
            UpdateCurrentLevelText();
        }

        private void UpdateScoreBar()
            => scoreBar.Initialize(0F, CurrentLevelSettings.totalScore);

        private void UpdateCurrentLevelText()
            => currentLevel.text = CurrentLevelSettings.name;
    }
}
