using System;
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
    public sealed class MatchPiece : MonoBehaviour, IComparable, IEquatable<MatchPiece>
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
        public MatchBoard Board { get; private set; }

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

        public float Width => Size.x;

        public float Height => Size.y;

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
        public bool Equals(MatchPiece other)
        {
            var otherId = other ? other.GetId() : -1;
            return GetId() == otherId;
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

            return CompareUsingHorizontalBoardPosition(otherPiece);
        }

        /// <summary>
        /// Compares using the current points.
        /// </summary>
        /// <param name="otherPiece">Other piece to compare.</param>
        /// <returns></returns>
        public int CompareUsingScorePoints(MatchPiece otherPiece)
            => scorePoints.CompareTo(otherPiece.GetPoints());

        public int CompareUsingHorizontalBoardPosition(MatchPiece otherPiece)
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

        public YieldInstruction Spawn(float duration)
        {
            transform.localScale = Vector3.one * 2F;
            return transform.DOScale(1F, duration).WaitForCompletion();
        }

        public Tween Move(Vector2 position)
            => transform.DOMove(position, duration: 0.25F);

        public YieldInstruction DropDown(Vector2 position)
            => transform.DOMove(position, duration: 0.1F).WaitForCompletion();

        public YieldInstruction ScaleDown()
            => transform.DOScale(0F, duration: 0.15F).WaitForCompletion();

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
        internal void SetBoard(MatchBoard board) => Board = board;

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
