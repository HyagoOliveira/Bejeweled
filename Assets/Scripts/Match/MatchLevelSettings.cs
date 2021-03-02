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

        /// <summary>
        /// The pieces count available for this level.
        /// </summary>
        public int PiecesCount => pieces.Length;
    }
}
