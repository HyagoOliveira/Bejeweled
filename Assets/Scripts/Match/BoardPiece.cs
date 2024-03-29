﻿using System;
using UnityEngine;
using DG.Tweening;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Component representing a Match Piece.
    /// <para>Use the <see cref="Place(Vector2Int, Vector2)"/> function to place it on the match board.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BoardPiece : MonoBehaviour, IComparable, IEquatable<BoardPiece>
    {
        [SerializeField, Tooltip("The local BoxCollider2D component.")]
        private BoxCollider2D boxCollider;
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

        /// <summary>
        /// The board this piece belongs to.
        /// </summary>
        public Board Board { get; private set; }

        /// <summary>
        /// Rendering color for the piece sprite.
        /// </summary>
        public Color Color
        {
            get => spriteRenderer.color;
            set => spriteRenderer.color = value;
        }

        /// <summary>
        /// Is this piece current selected?
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// The width and height of the piece.
        /// </summary>
        public Vector2 Size
        {
            get => boxCollider.size;
            set => boxCollider.size = value;
        }

        private bool wasSwapSelection;
        private Vector3 initialSelectionPosition;

        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseDown()
        {
            Board.HighlightPiece(this);
            initialSelectionPosition = Input.mousePosition;
        }

        private void OnMouseDrag() => CheckSwapSelection();

        private void OnMouseUp()
        {
            if (!wasSwapSelection) Board.SelectPiece(this);
            wasSwapSelection = false;
            initialSelectionPosition = default;
        }

        /// <summary>
        /// Check if the given piece is the same.
        /// </summary>
        /// <param name="other">The other piece to check.</param>
        /// <returns>Whether the other piece is the same.</returns>
        public bool Equals(BoardPiece other)
        {
            var otherId = other ? other.GetId() : -1;
            return GetId() == otherId;
        }

        /// <summary>
        /// Compares to other instances.
        /// <para>It's useful to sort as list of 
        /// <see cref="BoardPiece"/> using its points.</para>
        /// </summary>
        /// <param name="obj">The other instance to compare.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherPiece = obj as BoardPiece;
            if (otherPiece == null)
            {
                throw new ArgumentException($"Object is not a {typeof(BoardPiece).Name}");
            }

            return CompareUsingHorizontalBoardPosition(otherPiece);
        }

        /// <summary>
        /// Compares this piece using its current points.
        /// </summary>
        /// <param name="otherPiece">Other piece to compare.</param>
        /// <returns></returns>
        public int CompareUsingScorePoints(BoardPiece otherPiece)
            => scorePoints.CompareTo(otherPiece.GetPoints());

        /// <summary>
        /// Compares this piece using its current horizontal board position..
        /// </summary>
        /// <param name="otherPiece">Other piece to compare.</param>
        /// <returns></returns>
        public int CompareUsingHorizontalBoardPosition(BoardPiece otherPiece)
            => BoardPosition.x.CompareTo(otherPiece.BoardPosition.x);

        /// <summary>
        /// The unique id for this piece based on the current sprite.
        /// </summary>
        public int GetId() => spriteRenderer.sprite.GetInstanceID();

        /// <summary>
        /// The current Score points.
        /// </summary>
        /// <returns></returns>
        public int GetPoints() => scorePoints;

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

        /// <summary>
        /// Create a spawn animation for this piece.
        /// </summary>
        /// <param name="duration">The animation time.</param>
        /// <returns></returns>
        public YieldInstruction Spawn(float duration)
        {
            transform.localScale = Vector3.one * 2F;
            return transform.DOScale(1F, duration).WaitForCompletion();
        }

        /// <summary>
        /// Create a move animation for this piece.
        /// </summary>
        /// <param name="position">The position to move.</param>
        /// <returns></returns>
        public Tween Move(Vector2 position)
            => transform.DOMove(position, duration: 0.25F);


        /// <summary>
        /// Create a dropping down animation for this piece.
        /// </summary>
        /// <param name="rows">The number of rows to drop it down.</param>
        /// <returns></returns>
        public YieldInstruction DropDown(int rows)
        {
            var position = transform.position + Vector3.down * rows;
            var duration = rows * 0.06F;
            return transform.DOMove(position, duration).SetEase(Ease.InOutBounce).WaitForCompletion();
        }

        /// <summary>
        /// Create a scale down animation for this piece.
        /// </summary>
        /// <returns></returns>
        public YieldInstruction ScaleDown()
            => transform.DOScale(0F, duration: 0.15F).WaitForCompletion();

        /// <summary>
        /// Create a shake animation for this piece.
        /// </summary>
        /// <returns></returns>
        public Tween Shake()
            => transform.DOShakePosition(duration: 0.25F, strength: 0.5F);

        /// <summary>
        /// Selects this piece if possible.
        /// </summary>
        public void Select() => IsSelected = true;

        /// <summary>
        /// Unselect this piece.
        /// </summary>
        public void Unselect() => IsSelected = false;

        /// <summary>
        /// Sets the board.
        /// </summary>
        /// <param name="board">The board this piece belongs to.</param>
        internal void SetBoard(Board board) => Board = board;

        private void UpdateGameObjectName()
            => gameObject.name = $"{BoardPosition}\t{PrefabName}";

        private void CheckSwapSelection()
        {
            if (wasSwapSelection) return;

            const float swapThreshold = 0.5F;

            var currentMousePosition = Input.mousePosition;
            var dragDistance = Vector2.Distance(currentMousePosition, initialSelectionPosition);
            var dragWorldDistance = dragDistance / spriteRenderer.sprite.pixelsPerUnit;
            wasSwapSelection = Mathf.Abs(dragWorldDistance) > swapThreshold;

            if (wasSwapSelection)
            {
                var delta = currentMousePosition - initialSelectionPosition;
                var direction = delta.normalized;
                Board.SwapPieces(this, direction);
            }
        }
    }
}
