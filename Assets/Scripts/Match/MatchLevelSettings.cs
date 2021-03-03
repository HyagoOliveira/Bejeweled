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

        /// <summary>
        /// The pieces count available for this level.
        /// </summary>
        public int PiecesCount => pieces.Length;
    }
}
