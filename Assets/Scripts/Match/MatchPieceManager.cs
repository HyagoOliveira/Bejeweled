using UnityEngine;
using System.Collections.Generic;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Manager class responsible for instantiate <see cref="MatchPiece"/> components.
    /// They can be instantiated randomly.
    /// </summary>
    public sealed class MatchPieceManager
    {
        private readonly Dictionary<int, MatchPiece> prefabPieces = new Dictionary<int, MatchPiece>();

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        /// <param name="prefabs">A prefab array where each GameObject contains a <see cref="MatchPiece"/> component attached on it.</param>
        public MatchPieceManager(GameObject[] prefabs)
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
        /// <returns>A <see cref="MatchPiece"/> instance.</returns>
        public MatchPiece InstantiateRandomPiece(Transform parent)
        {
            var ids = new List<int>(prefabPieces.Keys);
            return InstantiateRandomPiece(parent, ids);
        }

        /// <summary>
        /// Instantiates a random match piece where the given invalidIds list are not used in the random selection.
        /// </summary>
        /// <param name="parent">The parent transform for this piece.</param>
        /// <param name="invalidIds">A list of invalid ids to NOT use when choose a random piece.</param>
        /// <returns>A <see cref="MatchPiece"/> instance.</returns>
        public MatchPiece InstantiateRandomPieceWithoutIds(Transform parent, int[] invalidIds)
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
        /// <returns>A <see cref="MatchPiece"/> instance.</returns>
        public MatchPiece InstantiateRandomPiece(Transform parent, List<int> ids)
        {
            var randomIdIndex = Random.Range(0, ids.Count);
            var id = ids[randomIdIndex];
            return InstantiatePiece(parent, id);
        }

        /// <summary>
        /// Instantiates a match piece using the given id.
        /// </summary>
        /// <param name="parent">The parent transform for this piece.</param>
        /// <param name="id">The piece id.</param>
        /// <returns>A <see cref="MatchPiece"/> instance.</returns>
        public MatchPiece InstantiatePiece(Transform parent, int id)
        {
            var invalidPieceID = !ContainsPiece(id);
            if (invalidPieceID) return null;

            var prefab = prefabPieces[id];
            var instance = Object.Instantiate(prefab, parent: parent);
            var piece = instance.GetComponent<MatchPiece>();
            if (piece) piece.PrefabName = prefab.name;
            return piece;
        }

        private void CreatePieceDictionary(GameObject[] prefabs)
        {
            prefabPieces.Clear();
            foreach (var prefab in prefabs)
            {
                var piece = prefab.GetComponent<MatchPiece>();
                if (piece) prefabPieces.Add(piece.GetId(), piece);
            }
        }
    }
}