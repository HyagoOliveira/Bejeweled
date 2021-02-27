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
        [SerializeField, Min(1), Tooltip("The number of rows for the level board.")]
        private int rows = 8;
        [SerializeField, Min(1), Tooltip("The number of columns for the level board.")]
        private int columns = 8;

        [Header("Tiles")]
        [SerializeField, Tooltip("Match piece prefabs for this level.")]
        private GameObject[] piecePrefabs = new GameObject[0];

        /// <summary>
        /// The board size based on the current 
        /// <see cref="rows"/> and <see cref="columns"/>.
        /// </summary>
        public Vector2Int BoardSize => new Vector2Int(columns, rows);

        /// <summary>
        /// Match piece prefabs for this level.
        /// </summary>
        public GameObject[] PiecePrefabs => piecePrefabs;
    }
}