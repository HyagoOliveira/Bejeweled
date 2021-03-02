using System;
using UnityEngine;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Component representing a Match Piece.
    /// <para>Use the <see cref="Place(Vector2Int, Vector2)"/> function to place it on the match board.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class MatchPiece : MonoBehaviour, IComparable, IEquatable<MatchPiece>
    {
        [SerializeField, Tooltip("The local SpriteRenderer component.")]
        private SpriteRenderer spriteRenderer;
        [SerializeField, Tooltip("The total Score points.")]
        private int scorePoints = 1;

        /// <summary>
        /// The prefab name used to create this instance.
        /// </summary>
        public string PrefabName { get; set; }

        /// <summary>
        /// The board position where this piece is.
        /// </summary>
        public Vector2Int BoardPosition { get; private set; }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Compares to other instances.
        /// <para>It's useful to sort as list of 
        /// <see cref="MatchPiece"/> using its points.</para>
        /// </summary>
        /// <param name="obj">The other instance to compare.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherPiece = obj as MatchPiece;
            if (otherPiece == null)
            {
                throw new ArgumentException("Object is not a MatchPiece");
            }

            return ComparePoints(otherPiece);
        }

        /// <summary>
        /// Check if the given piece is the same.
        /// </summary>
        /// <param name="other">The other piece to check.</param>
        /// <returns>Whether the other piece is the same.</returns>
        public bool Equals(MatchPiece other) => GetId() == other.GetId();

        /// <summary>
        /// The current Score points.
        /// </summary>
        /// <returns></returns>
        public int GetPoints() => scorePoints;

        /// <summary>
        /// Compares using the current points.
        /// </summary>
        /// <param name="otherPiece">Other piece to compare.</param>
        /// <returns></returns>
        public int ComparePoints(MatchPiece otherPiece)
            => scorePoints.CompareTo(otherPiece.GetPoints());

        /// <summary>
        /// The unique id for this piece based on the current sprite.
        /// </summary>
        public int GetId() => spriteRenderer.sprite.GetInstanceID();

        /// <summary>
        /// Places this piece using the given position.
        /// </summary>
        /// <param name="boardPosition">The board position to place.</param>
        /// <param name="position">The local position to place.</param>
        public void Place(Vector2Int boardPosition, Vector2 position)
        {
            var normalizedPivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;

            BoardPosition = boardPosition;
            transform.localPosition = position + normalizedPivot;

            UpdateGameObjectName();
        }

        private void UpdateGameObjectName()
            => gameObject.name = $"{BoardPosition}\t{PrefabName}";
    }
}