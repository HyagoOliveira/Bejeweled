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
            spriteRenderer = GetComponent<SpriteRenderer>();
            pieces = transform.Find("Pieces");
        }

        private void Awake()
        {
            Initialize(levelSettings);
        }

        /// <summary>
        /// Initializes the board and populate it.
        /// </summary>
        /// <param name="level">The level settings asset used to create this board.</param>
        public void Initialize(MatchLevelSettings level)
        {
            Board = new MatchPiece[level.BoardSize.x, level.BoardSize.y];
            PieceManager = new MatchPieceManager(level.Pieces);

            ResizeSpriteTile();
            Populate();
        }

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
        /// <returns>A piece id or -1 if the given position is invalid.</returns>
        public int GetPieceIdAt(int x, int y)
        {
            var validHorzPos = x > 0 && x < levelSettings.BoardSize.x;
            var validVertPos = y > 0 && y < levelSettings.BoardSize.y;
            var validBoardPos = validHorzPos && validVertPos;

            if (validBoardPos)
            {
                var piece = Board[x, y];
                return piece.GetId();
            }

            return -1;
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
            if (!HasFirstPieceSelected()) FirstPieceSelected = piece;
            else if (!HasSecondPieceSelected())
            {
                DisablePieceSelection();
                SecondPieceSelected = piece;
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

        private void ResizeSpriteTile()
        {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = GetSize();
        }

        [ContextMenu("Repopulate Board", isValidateFunction: true)]
        private bool IsPlayingMode() => Application.isPlaying;
    }
}