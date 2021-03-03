using UnityEngine;
using Bejeweled.UI;

namespace Bejeweled.Macth
{
    [DisallowMultipleComponent]
    public sealed class MatchLevelManager : MonoBehaviour
    {
        [SerializeField, Tooltip("The level settings assets.")]
        private MatchLevelSettings[] levelSettings = new MatchLevelSettings[0];

        public MatchLevelSettings CurrentLevelSettings
            => levelSettings[currentLevelSettingsIndex];

        public int LevelsCount => levelSettings.Length;

        private MatchBoard board;
        private SliderBar scoreBar;

        private int currentLevelSettingsIndex = 0;

        private void Awake()
        {
            board = FindObjectOfType<MatchBoard>();
            scoreBar = FindObjectOfType<SliderBar>();

            UpdateScoreBar();
        }

        private void OnEnable()
        {
            if (board) board.OnIncreaseScore += IncreaseScore;
        }

        private void OnDisable()
        {
            if (board) board.OnIncreaseScore -= IncreaseScore;
        }

        public void NextLevel()
        {
            currentLevelSettingsIndex++;
            if (currentLevelSettingsIndex >= LevelsCount)
                currentLevelSettingsIndex = 0;

            UpdateScoreBar();
        }

        public void PreviousLevel()
        {
            currentLevelSettingsIndex--;
            if (currentLevelSettingsIndex < 0)
                currentLevelSettingsIndex = LevelsCount - 1;

            UpdateScoreBar();
        }

        private void IncreaseScore(int score)
        {
            if (scoreBar) scoreBar.CurrentValue += score;
        }

        private void UpdateScoreBar()
        {
            if (!scoreBar) return;
            scoreBar.Initialize(0F, CurrentLevelSettings.totalScore);
        }
    }
}
