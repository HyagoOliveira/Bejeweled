using UnityEngine;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Container data for a match board.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBoardSettings", menuName = "Bejeweled/Match/Board Settings", order = 110)]
    public sealed class BoardSettings : ScriptableObject
    {
        [Header("Board")]
        [Tooltip("The total grid size for the board.")]
        public Vector2Int size = Vector2Int.one * 8;
        [Tooltip("The piece prefabs available for the board.")]
        public GameObject[] pieces = new GameObject[0];

        [Header("Gameplay")]
        [Min(1), Tooltip("Score necessary to go to next level.")]
        public float totalScore = 50;
        [Min(0f), Tooltip("Time (in seconds) to spawn pieces when populate the board.")]
        public float populateSpawnTime = 0.02f;
        [Tooltip("Revert to the last move if the swap does not result in any match sequence.")]
        public bool revertIfNoMatch = true;
        [Tooltip("Fill any empty spots after pieces are deleted.")]
        public bool fillEmptySpots = true;

        /// <summary>
        /// The piece prefabs available for the boardl.
        /// </summary>
        public int PiecesCount => pieces.Length;
    }
}
