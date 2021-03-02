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
        [SerializeField, Tooltip("The total grid size for the level board.")]
        private Vector2Int boardSize = Vector2Int.one * 8;
        [SerializeField, Tooltip("The piece prefabs available for this level.")]
        private GameObject[] pieces = new GameObject[0];

        /// <summary>
        /// The total grid size for the level board
        /// </summary>
        public Vector2Int BoardSize => boardSize;

        /// <summary>
        /// The piece prefabs available for this level.
        /// </summary>
        public GameObject[] Pieces => pieces;

        /// <summary>
        /// The pieces count available for this level.
        /// </summary>
        public int PiecesCount => Pieces.Length;
    }
}