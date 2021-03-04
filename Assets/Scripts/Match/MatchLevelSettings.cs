using UnityEngine;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Container data for a Match Level.
    /// </summary>
    [CreateAssetMenu(fileName = "MatchLevel", menuName = "Bejeweled/Match/Match Level Settings", order = 110)]
    public sealed class MatchLevelSettings : ScriptableObject
    {
        [Header("Board")]
        [Tooltip("The total grid size for the level board.")]
        public Vector2Int boardSize = Vector2Int.one * 8;
        [Tooltip("The piece prefabs available for this level.")]
        public GameObject[] pieces = new GameObject[0];

        [Header("Gameplay")]
        [Min(1), Tooltip("Score necessary to go to next level.")]
        public float totalScore = 50;
        [Min(0f), Tooltip("Time (in seconds) to swap between pieces.")]
        public float swapTime = 0.25f;
        [Min(0f), Tooltip("Time (in seconds) to remove matched pieces.")]
        public float removeTime = 0.15f;
        [Min(0f), Tooltip("Time (in seconds) to drop down a piece.")]
        public float dropDownTime = 0.08f;
        [Tooltip("Revert to the last move if the swap does not result in any match sequence.")]
        public bool revertIfNoMatch = true;

        /// <summary>
        /// The pieces count available for this level.
        /// </summary>
        public int PiecesCount => pieces.Length;
    }
}
