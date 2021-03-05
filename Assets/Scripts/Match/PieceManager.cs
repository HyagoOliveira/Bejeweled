using UnityEngine;
using System.Collections.Generic;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Manager class responsible for instantiate <see cref="BoardPiece"/> components.
    /// They can be instantiated randomly.
    /// </summary>
    public sealed class PieceManager
    {
        private readonly Dictionary<int, BoardPiece> prefabPieces = new Dictionary<int, BoardPiece>();

        /// <summary>
        /// The total pieces count.
        /// </summary>
        public int PiecesCount => prefabPieces.Count;

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        /// <param name="prefabs">A prefab array where each GameObject contains a <see cref="BoardPiece"/> component attached on it.</param>
        public PieceManager(GameObject[] prefabs)
            => CreatePieceDictionary(prefabs);

        /// <summary>
        /// Checks if the given piece id is available.
        /// </summary>
        /// <param name="id">The piece id.</param>
        /// <returns>True if the manager contains the id. False otherwise.</returns>
        public bool ContainsPiece(int id) => prefabPieces.ContainsKey(id);

        /// <summary>
        /// Instantiates a random match piece.
        /// </summary>
        /// <param name="parent">The parent transform for this piece.</param>
        /// <returns>A <see cref="BoardPiece"/> instance.</returns>
        public BoardPiece InstantiateRandomPiece(Transform parent)
        {
            var ids = new List<int>(prefabPieces.Keys);
            return InstantiateRandomPiece(parent, ids);
        }

        /// <summary>
        /// Instantiates a random match piece where the given invalidIds list are not used in the random selection.
        /// </summary>
        /// <param name="parent">The parent transform for this piece.</param>
        /// <param name="invalidIds">A list of invalid ids to NOT use when choose a random piece.</param>
        /// <returns>A <see cref="BoardPiece"/> instance.</returns>
        public BoardPiece InstantiateRandomPieceWithoutIds(Transform parent, int[] invalidIds)
        {
            var ids = new List<int>(prefabPieces.Keys);
            foreach (var invalidId in invalidIds)
            {
                ids.Remove(invalidId);
            }

            return InstantiateRandomPiece(parent, ids);
        }

        /// <summary>
        /// Instantiates a random match piece using the given ids list.
        /// </summary>
        /// <param name="parent">The parent transform for this piece.</param>
        /// <param name="ids">A list of valid ids to use when choose a random piece.</param>
        /// <returns>A <see cref="BoardPiece"/> instance.</returns>
        public BoardPiece InstantiateRandomPiece(Transform parent, List<int> ids)
        {
            if (ids.Count == 0) return InstantiateRandomPiece(parent);

            var randomIdIndex = Random.Range(0, ids.Count);
            var id = ids[randomIdIndex];
            return InstantiatePiece(parent, id);
        }

        /// <summary>
        /// Instantiates a match piece using the given id.
        /// </summary>
        /// <param name="parent">The parent transform for this piece.</param>
        /// <param name="id">The piece id.</param>
        /// <returns>A <see cref="BoardPiece"/> instance.</returns>
        public BoardPiece InstantiatePiece(Transform parent, int id)
        {
            var invalidPieceID = !ContainsPiece(id);
            if (invalidPieceID) return null;

            var prefab = prefabPieces[id];
            var instance = Object.Instantiate(prefab, parent: parent);
            var piece = instance.GetComponent<BoardPiece>();
            if (piece) piece.PrefabName = prefab.name;
            return piece;
        }

        private void CreatePieceDictionary(GameObject[] prefabs)
        {
            prefabPieces.Clear();
            foreach (var prefab in prefabs)
            {
                var piece = prefab.GetComponent<BoardPiece>();
                if (piece) prefabPieces.Add(piece.GetId(), piece);
            }
        }
    }
}
