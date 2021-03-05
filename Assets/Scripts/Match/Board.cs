using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Component responsible for creating and managing the Match Board.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoardSounds))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Board : MonoBehaviour
    {
        [SerializeField, Tooltip("The level settings asset.")]
        private BoardSettings levelSettings;
        [SerializeField, Tooltip("The local BoardSounds component.")]
        private BoardSounds sounds;
        [SerializeField, Tooltip("The local SpriteRenderer component.")]
        private SpriteRenderer spriteRenderer;
        [SerializeField, Tooltip("The child Transform to hold all the board pieces.")]
        private Transform piecesParent;
        [SerializeField, Tooltip("The child Transform for the board selector.")]
        private Transform selector;

        /// <summary>
        /// The board grid array.
        /// </summary>
        public BoardPiece[,] Pieces { get; private set; }

        /// <summary>
        /// The manager used to populate this board.
        /// </summary>
        public PieceManager PieceManager { get; private set; }

        /// <summary>
        /// Is able to swap pieces in this board?
        /// </summary>
        public bool CanSwapPieces { get; private set; }

        /// <summary>
        /// The current selected piece at this board.
        /// </summary>
        public BoardPiece SelectedPiece { get; private set; }

        /// <summary>
        /// The local BoardSounds component.
        /// </summary>
        public BoardSounds Sounds => sounds;

        /// <summary>
        /// Action executed every time a match is done and the score increases.
        /// </summary>
        public Action<int> OnIncreaseScore { get; set; }

        private void Reset()
        {
            piecesParent = transform.Find("Pieces");
            selector = transform.Find("BoardSelector");
            sounds = GetComponent<BoardSounds>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            if (levelSettings) Populate();
        }

        /// <summary>
        /// Populates the board using the given level settings asset.
        /// </summary>
        /// <param name="levelSettings">Level Settings used to populate the board.</param>
        public void Populate(BoardSettings levelSettings)
        {
            this.levelSettings = levelSettings;
            Populate();
        }

        public void HighlightPiece(BoardPiece piece)
        {
            var shouldHighlight = !HasSelectedPiece();
            if (shouldHighlight) MoveSelectorToPiece(piece);
        }

        /// <summary>
        /// Removes all <see cref="BoardPiece"/> GameObjects from this board.
        /// </summary>
        public void RemoveAllPieces()
        {
            var piecesComponents = piecesParent.GetComponentsInChildren<BoardPiece>(includeInactive: true);
            foreach (var piece in piecesComponents)
            {
                Destroy(piece.gameObject);
            }
        }

        /// <summary>
        /// Returns the board center position.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> position.</returns>
        public Vector2Int GetCenterPosition()
            => new Vector2Int(
                (int)transform.localPosition.x,
                (int)transform.localPosition.y);

        /// <summary>
        /// Returns the board bottom left position.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> position.</returns>
        public Vector2Int GetBottomLeftPosition()
            => GetCenterPosition() - GetSize() / 2;

        /// <summary>
        /// Returns the board size.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int GetSize()
            => new Vector2Int(GetWidth(), GetHeight());

        /// <summary>
        /// Returns the board sprite tiled size.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int GetSpriteTiledSize()
        {
            return new Vector2Int(
                (int)spriteRenderer.size.x,
                (int)spriteRenderer.size.y);
        }

        public int GetWidth() => Pieces.GetLength(0);

        public int GetHeight() => Pieces.GetLength(1);

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
        /// Returns the piece at the given board position.
        /// </summary>
        /// <param name="boardPosition">The board position.</param>
        /// <returns>A piece instance or null if the given board position is outside the board.</returns>
        public BoardPiece GetPieceAt(Vector2Int boardPosition)
        {
            if (boardPosition == null) return null;
            return GetPieceAt(boardPosition.x, boardPosition.y);
        }

        /// <summary>
        /// Returns the piece at the given position.
        /// </summary>
        /// <param name="x">The horizontal board position.</param>
        /// <param name="y">The vertical board position.</param>
        /// <returns>A piece instance or null if the given position is outside the board.</returns>
        public BoardPiece GetPieceAt(int x, int y)
        {
            var validHorzPos = x >= 0 && x < levelSettings.size.x;
            var validVertPos = y >= 0 && y < levelSettings.size.y;
            var validBoardPos = validHorzPos && validVertPos;
            return validBoardPos ? Pieces[x, y] : null;
        }

        public Vector2Int GetDroppedPosition(BoardPiece piece, out int droppedRows)
        {
            droppedRows = 0;
            var droppedPosition = piece.BoardPosition;
            while (CanDropDownPieceAt(droppedPosition))
            {
                droppedRows++;
                droppedPosition += Vector2Int.down;
                if (droppedRows > GetHeight()) break;
            }

            return droppedPosition;
        }

        /// <summary>
        /// Set the given piece at the board position.
        /// </summary>
        /// <param name="boardPosition">The board position.</param>
        /// <param name="piece">The piece to set.</param>
        public void SetPieceAt(Vector2Int boardPosition, BoardPiece piece)
        {
            var position = GetBottomLeftPosition() + boardPosition;
            Pieces[boardPosition.x, boardPosition.y] = piece;
            Pieces[boardPosition.x, boardPosition.y].Place(boardPosition, position);
        }

        public void HidePieceAt(Vector2Int boardPosition)
        {
            Pieces[boardPosition.x, boardPosition.y].gameObject.SetActive(false);
        }

        public void DestroyPieceAt(Vector2Int boardPosition)
        {
            var cannotDestroy = !HasPieceAt(boardPosition);
            if (cannotDestroy) return;

            var gameObject = Pieces[boardPosition.x, boardPosition.y].gameObject;
            Pieces[boardPosition.x, boardPosition.y] = null;
            Destroy(gameObject);
        }

        /// <summary>
        /// Enables the piece swap.
        /// </summary>
        public void EnablePieceSwap() => CanSwapPieces = true;

        /// <summary>
        /// Disable the piece swap.
        /// </summary>
        public void DisablePieceSwap() => CanSwapPieces = false;

        /// <summary>
        /// Enable the board selector.
        /// </summary>
        public void EnableSelector() => selector.gameObject.SetActive(true);

        /// <summary>
        /// Disable the board selector.
        /// </summary>
        public void DisableSelector() => selector.gameObject.SetActive(false);

        /// <summary>
        /// Moves the board selector to the given piece board position.
        /// </summary>
        /// <param name="piece">The piece to move the selector.</param>
        public void MoveSelectorToPiece(BoardPiece piece)
        {
            EnableSelector();
            selector.position = piece.transform.position;
        }

        /// <summary>
        /// Selects the given piece and execute all match logic.
        /// </summary>
        /// <param name="piece">A piece to select.</param>
        public void SelectPiece(BoardPiece piece)
        {
            if (CanSwapPieces)
            {
                if (HasSelectedPiece()) SelectAsSecondPiece(piece);
                else SelectAsFirstPiece(piece);
            }
        }

        /// <summary>
        /// Unselects the <see cref="SelectedPiece"/>.
        /// </summary>
        public void UnselectPiece()
        {
            DisableSelector();
            SelectedPiece.Unselect();
            SelectedPiece = null;
        }

        public void SwapPieces(BoardPiece piece, Vector2 direction)
        {
            var boardDirection = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
            var otherPieceBoardPosition = piece.BoardPosition + boardDirection;
            var otherPiece = GetPieceAt(otherPieceBoardPosition);
            var hasOtherPiece = otherPiece != null;
            if (hasOtherPiece)
            {
                SelectedPiece = piece;
                StartCoroutine(CheckMatchesAndFillBoard(otherPiece));
            }
            else piece.Shake();
        }

        /// <summary>
        /// Swaps the given pieces.
        /// </summary>
        /// <param name="piece">A piece to swap.</param>
        /// <param name="otherPiece">A piece to swap.</param>
        public IEnumerator SwapPieces(BoardPiece piece, BoardPiece otherPiece)
        {
            DisablePieceSwap();
            var swapSequence = DOTween.Sequence().
                Append(piece.Move(otherPiece.transform.position)).
                Join(otherPiece.Move(piece.transform.position));

            yield return swapSequence.WaitForCompletion();

            var secondPosition = otherPiece.BoardPosition;
            SetPieceAt(piece.BoardPosition, otherPiece);
            SetPieceAt(secondPosition, piece);
            EnablePieceSwap();
        }

        /// <summary>
        /// Checks if the given piece is selected.
        /// </summary>
        /// <param name="piece">The piece to check.</param>
        /// <returns>True if the given piece is selected. False otherwise.</returns>
        public bool IsSelectedPiece(BoardPiece piece) => piece.gameObject.Equals(SelectedPiece.gameObject);

        /// <summary>
        /// Checks if the board has a selected piece.
        /// </summary>
        /// <returns>True if the board has selected piece. False otherwise.</returns>
        public bool HasSelectedPiece() => SelectedPiece != null;

        public bool HasPieceAt(Vector2Int position) => GetPieceAt(position) != null;

        public bool CanDropDownPiece(BoardPiece piece)
            => piece && CanDropDownPieceAt(piece.BoardPosition);

        public bool CanDropDownPieceAt(Vector2Int position)
        {
            if (position.y <= 0) return false;
            var bottomBoardPosition = position + Vector2Int.down;
            return GetPieceAt(bottomBoardPosition) == null;
        }

        /// <summary>
        /// Checks if the given pieces are orthogonally adjacent, 
        /// i.e. they are next to each other horizontally or vertically.
        /// </summary>
        /// <param name="piece">The first piece to check.</param>
        /// <param name="otherPiece">The second piece to check.</param>
        /// <returns>True if the given positions are adjacents to each other. False otherwise.</returns>
        public bool IsAdjacentPieces(BoardPiece piece, BoardPiece otherPiece)
            => IsAdjacentPosition(piece.BoardPosition, otherPiece.BoardPosition);

        /// <summary>
        /// Checks if the given positions are orthogonally adjacent, 
        /// i.e. they are next to each other horizontally or vertically.
        /// </summary>
        /// <param name="boardPos">The first board position to check.</param>
        /// <param name="otherBoardPos">The second board position to check.</param>
        /// <returns>True if the given positions are adjacents to each other. False otherwise.</returns>
        public bool IsAdjacentPosition(Vector2Int boardPos, Vector2Int otherBoardPos)
        {
            var isFromSameLine = boardPos.y == otherBoardPos.y;
            var isFromSameColumn = boardPos.x == otherBoardPos.x;
            var isHorzAdjacent = isFromSameLine && (boardPos.x == otherBoardPos.x + 1 || boardPos.x == otherBoardPos.x - 1);
            var isVertAdjacent = isFromSameColumn && (boardPos.y == otherBoardPos.y + 1 || boardPos.y == otherBoardPos.y - 1);
            return isHorzAdjacent || isVertAdjacent;
        }

        private void ResizeSpriteTile()
        {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = GetSize();
        }

        private void SelectAsFirstPiece(BoardPiece piece)
        {
            SelectedPiece = piece;
            SelectedPiece.Select();
        }

        private void SelectAsSecondPiece(BoardPiece piece)
        {
            if (IsSelectedPiece(piece))
            {
                UnselectPiece();
                return;
            }

            var shouldSelectAsFirstPiece = !IsAdjacentPieces(SelectedPiece, piece);
            if (shouldSelectAsFirstPiece)
            {
                SelectAsFirstPiece(piece);
                return;
            }

            StartCoroutine(CheckMatchesAndFillBoard(piece));
        }

        private IEnumerator CheckMatchesAndFillBoard(BoardPiece piece)
        {
            DisableSelector();
            yield return SwapPieces(SelectedPiece, piece);

            var matchedPieces = GetMatchedPieces(out bool wasMatch);
            var revertMove = levelSettings.revertIfNoMatch && !wasMatch;
            if (revertMove) yield return SwapPieces(piece, SelectedPiece);
            UnselectPiece();

            while (wasMatch)
            {
                yield return ComputerMatches(matchedPieces);
                yield return DropDownPieces();
                matchedPieces = GetMatchedPieces(out wasMatch);
            }

            yield return new WaitForSeconds(0.1f);
            EnablePieceSwap();
        }

        private IEnumerator ComputerMatches(List<BoardPiece> matchedPieces)
        {
            DisablePieceSwap();
            var totalScore = 0;
            foreach (var piece in matchedPieces)
            {
                totalScore += piece.GetPoints();
                //TODO play pop sound
                //TODO add score number animation
                yield return piece.ScaleDown();
            }

            foreach (var piece in matchedPieces)
            {
                DestroyPieceAt(piece.BoardPosition);
            }

            var hasScore = totalScore > 0;
            if (hasScore) OnIncreaseScore?.Invoke(totalScore);
        }

        private IEnumerator DropDownPieces()
        {
            DisablePieceSwap();
            var size = GetSize();

            for (int y = 1; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var boardPosition = new Vector2Int(x, y);
                    var currentPiece = GetPieceAt(boardPosition);
                    var cannotDropDown = !CanDropDownPiece(currentPiece);
                    if (cannotDropDown) continue;

                    var droppedBoardPosition = GetDroppedPosition(currentPiece, out int droppedRows);
                    var droppedWorldPosition = currentPiece.transform.position + Vector3.down * droppedRows;

                    //TODO play drop sound
                    yield return currentPiece.DropDown(droppedWorldPosition);
                    SetPieceAt(droppedBoardPosition, currentPiece);
                    Pieces[boardPosition.x, boardPosition.y] = null;
                }
            }

            if (levelSettings.fillEmptySpots)
            {
                yield return FillEmptySpots();
            }
        }

        private IEnumerator FillEmptySpots(float spawnTime = 0.1F)
        {
            DisablePieceSwap();
            var size = GetSize();
            var showAnimation = spawnTime > 0F;
            var bottomLeftPosition = GetBottomLeftPosition();

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var boardPosition = new Vector2Int(x, y);
                    var hasPiece = HasPieceAt(boardPosition);
                    if (hasPiece) continue;

                    var localPosition = bottomLeftPosition + boardPosition;
                    var invalidPieceIds = new int[]
                    {
                        GetPieceIdAt(x - 1, y), // Gets the closest left piece id from the current position.
                        GetPieceIdAt(x - 2, y), // Gets the further left piece id from the current position.
                        GetPieceIdAt(x, y - 1), // Gets the closest bottom piece id from the current position.
                        GetPieceIdAt(x, y - 2)  // Gets the further bottom piece id from the current position.
                    };

                    var currentPiece = PieceManager.InstantiateRandomPieceWithoutIds(piecesParent, invalidPieceIds);
                    currentPiece.SetBoard(this);
                    currentPiece.Place(boardPosition, localPosition);
                    Pieces[x, y] = currentPiece;

                    if (showAnimation)
                    {
                        Sounds.PlayPieceSpawn();
                        yield return currentPiece.Spawn(spawnTime);
                    }
                }
            }

            EnablePieceSwap();
        }

        private List<BoardPiece> GetMatchedPieces(out bool wasMatch)
        {
            var size = GetSize();
            var matchedPieces = new List<BoardPiece>();

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var boardPosition = new Vector2Int(x, y);
                    var currentPiece = GetPieceAt(boardPosition);
                    if (currentPiece == null) continue;

                    var vertMatches = FindVerticalMatches(currentPiece, out bool hasVertMatch);
                    var horzMatches = FindHorizontalMatches(currentPiece, out bool hasHorzMatch);

                    if (hasVertMatch)
                    {
                        matchedPieces.AddRange(vertMatches);
                        matchedPieces.Add(currentPiece);
                    }
                    if (hasHorzMatch)
                    {
                        matchedPieces.AddRange(horzMatches);
                        matchedPieces.Add(currentPiece);
                    }
                }
            }

            matchedPieces.Sort();
            wasMatch = matchedPieces.Count > 0;
            return matchedPieces;
        }

        private List<BoardPiece> FindVerticalMatches(BoardPiece piece, out bool wasMatch)
        {
            var rows = GetHeight();
            var matches = new List<BoardPiece>(capacity: rows);

            for (int y = piece.BoardPosition.y + 1; y < rows; y++)
            {
                var currentPiece = GetPieceAt(piece.BoardPosition.x, y);
                var noMatch = currentPiece == null || !currentPiece.Equals(piece);
                if (noMatch) break;

                matches.Add(currentPiece);
            }

            wasMatch = matches.Count > 1;
            return matches;
        }

        private List<BoardPiece> FindHorizontalMatches(BoardPiece piece, out bool wasMatch)
        {
            var columns = GetWidth();
            var matches = new List<BoardPiece>(capacity: columns);

            for (int x = piece.BoardPosition.x + 1; x < columns; x++)
            {
                var currentPiece = GetPieceAt(x, piece.BoardPosition.y);
                var noMatch = currentPiece == null || !currentPiece.Equals(piece);
                if (noMatch) break;

                matches.Add(currentPiece);
            }

            wasMatch = matches.Count > 1;
            return matches;
        }

        private const string POPULATE_BOARD_CONTX_MENU_TITLE = "Populate Board";

        [ContextMenu(POPULATE_BOARD_CONTX_MENU_TITLE)]
        private void Populate()
        {
            Pieces = new BoardPiece[levelSettings.size.x, levelSettings.size.y];
            PieceManager = new PieceManager(levelSettings.pieces);

            DisableSelector();
            ResizeSpriteTile();
            RemoveAllPieces();

            StartCoroutine(FillEmptySpots(levelSettings.populateSpawnTime));
        }

        [ContextMenu(POPULATE_BOARD_CONTX_MENU_TITLE, isValidateFunction: true)]
        private bool IsPlayingMode() => Application.isPlaying;
    }
}
