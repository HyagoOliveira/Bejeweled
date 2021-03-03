using UnityEngine;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Component responsible for creating and managing the Match Board.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class MatchBoard : MonoBehaviour
    {
        [SerializeField, Tooltip("The level settings asset.")]
        private MatchLevelSettings levelSettings;
        [SerializeField, Tooltip("The local SpriteRenderer component.")]
        private SpriteRenderer spriteRenderer;
        [SerializeField, Tooltip("The child Transform to hold all the board pieces.")]
        private Transform pieces;
        [SerializeField, Tooltip("The child Transform for the board selector.")]
        private Transform selector;

        /// <summary>
        /// The board grid array.
        /// </summary>
        public MatchPiece[,] Board { get; private set; }

        /// <summary>
        /// The board first selected piece.
        /// </summary>
        public MatchPiece FirstPieceSelected { get; private set; }

        /// <summary>
        /// The board second selected piece.
        /// </summary>
        public MatchPiece SecondPieceSelected { get; private set; }

        /// <summary>
        /// The manager used to populate this board.
        /// </summary>
        public MatchPieceManager PieceManager { get; private set; }

        /// <summary>
        /// Is able to select pieces in this board?
        /// </summary>
        public bool CanSelectPieces { get; private set; }

        private void Reset()
        {
            pieces = transform.Find("Pieces");
            selector = transform.Find("BoardSelector");
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake() => Initialize();

        /// <summary>
        /// Populates the board using <see cref="PieceManager"/>
        /// </summary>
        [ContextMenu("Repopulate Board")]
        public void Populate()
        {
            DisablePieceSelection();
            RemoveAllPieces();

            var size = GetSize();
            var halfSize = size / 2;
            var bottomLeftPosition = GetCenterPosition() - halfSize;

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var boardPosition = new Vector2Int(x, y);
                    var localPosition = bottomLeftPosition + boardPosition;
                    var invalidPieceIds = new int[]
                    {
                        GetPieceIdAt(x - 1, y), // Get the left piece id from the current position.
                        GetPieceIdAt(x - 2, y), // Get the leftmost piece id from the current position.
                        GetPieceIdAt(x, y - 1), // Get the bottom piece id from the current position.
                        GetPieceIdAt(x, y - 2)  // Get the bottommost piece id from the current position.
                    };

                    Board[x, y] = PieceManager.InstantiateRandomPieceWithoutIds(pieces, invalidPieceIds);
                    Board[x, y].SetBoard(this);
                    Board[x, y].Place(boardPosition, localPosition);
                }
            }

            EnablePieceSelection();
        }

        /// <summary>
        /// Removes all <see cref="MatchPiece"/> GameObjects from this board.
        /// </summary>
        public void RemoveAllPieces()
        {
            var piecesComponents = pieces.GetComponentsInChildren<MatchPiece>(includeInactive: true);
            foreach (var piece in piecesComponents)
            {
                Destroy(piece.gameObject);
            }
        }

        /// <summary>
        /// Returns the board center position.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int GetCenterPosition()
            => new Vector2Int(
                (int)transform.localPosition.x,
                (int)transform.localPosition.y);

        /// <summary>
        /// Returns the board size.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int GetSize()
            => new Vector2Int(Board.GetLength(0), Board.GetLength(1));

        /// <summary>
        /// Returns the board sprite tiled size.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int GetSpriteTileSize()
        {
            return new Vector2Int(
                (int)spriteRenderer.size.x,
                (int)spriteRenderer.size.y);
        }

        /// <summary>
        /// Returns the piece id at the given position.
        /// </summary>
        /// <param name="x">The horizontal board position.</param>
        /// <param name="y">The vertical board position.</param>
        /// <returns>A piece id or -1 if the given position is outside the board.</returns>
        public int GetPieceIdAt(int x, int y)
        {
            var piece = GetPieceAt(x, y);
            return piece ? piece.GetId() : -1;
        }

        /// <summary>
        /// Returns the piece at the given position.
        /// </summary>
        /// <param name="x">The horizontal board position.</param>
        /// <param name="y">The vertical board position.</param>
        /// <returns>A piece instance or null if the given position is outside the board.</returns>
        public MatchPiece GetPieceAt(int x, int y)
        {
            var validHorzPos = x >= 0 && x < levelSettings.boardSize.x;
            var validVertPos = y >= 0 && y < levelSettings.boardSize.y;
            var validBoardPos = validHorzPos && validVertPos;
            return validBoardPos ? Board[x, y] : null;
        }

        /// <summary>
        /// Enables the piece selection.
        /// </summary>
        public void EnablePieceSelection() => CanSelectPieces = true;

        /// <summary>
        /// Disable the piece selection.
        /// </summary>
        public void DisablePieceSelection() => CanSelectPieces = false;

        /// <summary>
        /// Selects the given piece.
        /// </summary>
        /// <param name="piece">A piece to select.</param>
        public void SelectPiece(MatchPiece piece)
        {
            if (!HasFirstPieceSelected()) SelectFirstPiece(piece);
            else if (!HasSecondPieceSelected())
            {
                var isAdjacent = IsAdjacentPosition(FirstPieceSelected.BoardPosition, piece.BoardPosition);
                if (isAdjacent) SelectSecondPiece(piece);
                else SelectFirstPiece(piece);
            }
        }

        /// <summary>
        /// Unselects the given piece.
        /// </summary>
        /// <param name="piece">A piece to unselect.</param>
        public void UnselectPiece(MatchPiece piece)
        {
            if (piece.Equals(FirstPieceSelected))
            {
                EnablePieceSelection();
                FirstPieceSelected = null;
            }
            else if (piece.Equals(SecondPieceSelected))
            {
                EnablePieceSelection();
                SecondPieceSelected = null;
            }
        }

        public void MoveSelectorToPiece(MatchPiece piece)
        {
            selector.position = piece.transform.position;
            EnableSelector(true);
        }

        public void EnableSelector(bool enabled) => selector.gameObject.SetActive(enabled);

        public void SelectFirstPiece(MatchPiece piece)
        {
            MoveSelectorToPiece(piece);
            FirstPieceSelected = piece;
        }

        public void SelectSecondPiece(MatchPiece piece)
        {
            EnableSelector(false);
            DisablePieceSelection();
            SecondPieceSelected = piece;
        }

        /// <summary>
        /// Checks if the board first piece is selected.
        /// </summary>
        /// <returns>True if the board has the first piece selected. False otherwise.</returns>
        public bool HasFirstPieceSelected() => FirstPieceSelected != null;

        /// <summary>
        /// Checks if the board second piece is selected.
        /// </summary>
        /// <returns>True if the board has the second piece selected. False otherwise.</returns>
        public bool HasSecondPieceSelected() => SecondPieceSelected != null;

        /// <summary>
        /// Checks if the given positions are orthogonally adjacent, 
        /// i.e. they are next to each other horizontally or vertically.
        /// </summary>
        /// <param name="firstPosition">The first position to check.</param>
        /// <param name="secondPosition">The second position to check.</param>
        /// <returns>True if the given positions are adjacents. False otherwise.</returns>
        public bool IsAdjacentPosition(Vector2Int firstPosition, Vector2Int secondPosition)
        {
            var isHorzAdjacent = firstPosition.x == secondPosition.x + 1 || firstPosition.x == secondPosition.x - 1;
            var isVertAdjacent = firstPosition.y == secondPosition.y + 1 || firstPosition.y == secondPosition.y - 1;
            return isHorzAdjacent || isVertAdjacent;
        }

        private void Initialize()
        {
            Board = new MatchPiece[levelSettings.boardSize.x, levelSettings.boardSize.y];
            PieceManager = new MatchPieceManager(levelSettings.pieces);

            EnableSelector(false);
            ResizeSpriteTile();
            Populate();
        }

        private void ResizeSpriteTile()
        {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = GetSize();
        }

        [ContextMenu("Repopulate Board", isValidateFunction: true)]
        private bool IsPlayingMode() => Application.isPlaying;
    }
}
