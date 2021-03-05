using UnityEngine;
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
        [SerializeField, Tooltip("The local SliderBar component.")]
        private SliderBar scoreBar;
        [SerializeField, Tooltip("The level settings assets.")]
        private MatchLevelSettings[] levelSettings = new MatchLevelSettings[0];

        /// <summary>
        /// The current MatchLevelSettings used to populate the board. 
        /// </summary>
        public MatchLevelSettings CurrentLevelSettings
            => levelSettings[currentLevelSettingsIndex];

        /// <summary>
        /// Total level count.
        /// </summary>
        public int LevelsCount => levelSettings.Length;

        private MatchBoard board;
        private int currentLevelSettingsIndex = 0;

        private void Awake()
        {
            board = FindObjectOfType<MatchBoard>();
            InitializeScoreBar();
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
            InitializeScoreBar();
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
            InitializeScoreBar();
        }

        private void IncreaseScore(int score) => scoreBar.CurrentValue += score;

        private void InitializeScoreBar()
            => scoreBar.Initialize(0F, CurrentLevelSettings.totalScore);
    }
}
