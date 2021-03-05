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
        /// The piece factory used to populate this board.
        /// </summary>
        public BoardPieceFactory PieceFactory { get; private set; }

        /// <summary>
        /// Is able to move pieces in this board?
        /// </summary>
        public bool CanMovePieces { get; private set; } = true;

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

        /// <summary>
        /// Highlights the given piece.
        /// </summary>
        /// <param name="piece">A piece to highlight on the board.</param>
        public void HighlightPiece(BoardPiece piece)
        {
            Sounds.PlayPieceSelection();
            MoveSelectorToPiece(piece);
        }

        /// <summary>
        /// Highlights a piece that can be matched.
        /// </summary>
        public void HighlightHint()
        {
            var preMatchPiece = GetPreMatchPiece();
            if (preMatchPiece)
            {
                MoveSelectorToPiece(preMatchPiece);
                Invoke("DisableSelector", time: 1F);
            }
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
        /// <returns>Always a <see cref="Vector2"/> position.</returns>
        public Vector2 GetBottomLeftPosition()
            => GetCenterPosition() - GetSize() * 0.5F;

        /// <summary>
        /// Returns the board size as integers.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int GetSizeAsInt()
            => new Vector2Int(GetWidth(), GetHeight());

        /// <summary>
        /// Returns the board size.
        /// </summary>
        /// <returns>Always a <see cref="Vector2"/> instance.</returns>
        public Vector2 GetSize()
            => new Vector2(GetWidth(), GetHeight());

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

        /// <summary>
        /// Gets the width of the board.
        /// </summary>
        /// <returns>Always a integer number.</returns>
        public int GetWidth() => Pieces.GetLength(0);

        /// <summary>
        /// Gets the height of the board.
        /// </summary>
        /// <returns>Always a integer number.</returns>
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

        /// <summary>
        /// Finds a piece that can be matched.
        /// </summary>
        /// <returns>A piece that can be matched or null.</returns>
        public BoardPiece GetPreMatchPiece()
        {
            var size = GetSizeAsInt() - Vector2Int.one;

            for (int y = 1; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var pos = new Vector2Int(x, y);
                    var piece = GetPieceAt(pos);
                    if (piece == null) continue;

                    var rightPieces = GetPieces(pos, Vector2Int.right, 2);
                    var bottomPieces = GetPieces(pos, Vector2Int.down, 2);
                    var isRightDoubleMatch = rightPieces[0] && rightPieces[1] && rightPieces[0].Equals(rightPieces[1]);
                    var isBottomDoubleMatch = bottomPieces[0] && bottomPieces[1] && bottomPieces[0].Equals(bottomPieces[1]);
                    var canHorizontalMatch = isRightDoubleMatch &&
                        (
                            IsSamePiece(rightPieces[0], pos + Vector2Int.up) ||
                            IsSamePiece(rightPieces[0], pos + Vector2Int.left) ||
                            IsSamePiece(rightPieces[0], pos + Vector2Int.down)
                        );
                    var canVerticalMatch = isBottomDoubleMatch &&
                        (
                            IsSamePiece(bottomPieces[0], pos + Vector2Int.left) ||
                            IsSamePiece(bottomPieces[0], pos + Vector2Int.up) ||
                            IsSamePiece(bottomPieces[0], pos + Vector2Int.right)
                        );
                    var canMatch = canHorizontalMatch || canVerticalMatch;
                    if (canMatch) return piece;

                    var canBottomMatch =
                        IsSamePiece(piece, piece.BoardPosition + new Vector2Int(1, -1)) &&
                        IsSamePiece(piece, piece.BoardPosition + new Vector2Int(-1, -1));
                    if (canBottomMatch) return piece;
                }
            }

            return null;
        }


        /// <summary>
        /// Get a list of pieces from the given position and direction.
        /// </summary>
        /// <param name="position">The position to fetch the list. This position is not included in the list.</param>
        /// <param name="direction">The direction to fetch the list.</param>
        /// <param name="count">The number of pieces to fetch.</param>
        /// <returns></returns>
        public BoardPiece[] GetPieces(Vector2Int position, Vector2Int direction, int count)
        {
            var pieces = new BoardPiece[count];
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i] = GetPieceAt(position + direction * (i + 1));
            }
            return pieces;
        }

        /// <summary>
        /// Finds the position where the given piece should be dropped.
        /// </summary>
        /// <param name="piece">The piece to drop.</param>
        /// <param name="droppedRows">The number of dropped rows.</param>
        /// <returns>The position where the given piece should be dropped.</returns>
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

        /// <summary>
        /// Destroys the a piece placed at the given position. Nothing get destroyed if no piece is found.
        /// </summary>
        /// <param name="boardPosition">The board position to destroy the piece.</param>
        public void DestroyPieceAt(Vector2Int boardPosition)
        {
            var cannotDestroy = !HasPieceAt(boardPosition);
            if (cannotDestroy) return;

            var gameObject = Pieces[boardPosition.x, boardPosition.y].gameObject;
            Pieces[boardPosition.x, boardPosition.y] = null;
            Destroy(gameObject);
        }

        /// <summary>
        /// Enables the piece movement.
        /// </summary>
        public void EnablePieceMovement() => CanMovePieces = true;

        /// <summary>
        /// Disable the piece movement.
        /// </summary>
        public void DisablePieceMovement() => CanMovePieces = false;

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
            if (CanMovePieces)
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

        /// <summary>
        /// Swaps the given piece using the direction.
        /// </summary>
        /// <param name="piece">The piece to swap.</param>
        /// <param name="direction">The normalized direction to swaps the piece.</param>
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
            DisablePieceMovement();
            var swapSequence = DOTween.Sequence().
                Append(piece.Move(otherPiece.transform.position)).
                Join(otherPiece.Move(piece.transform.position));

            yield return swapSequence.WaitForCompletion();

            var secondPosition = otherPiece.BoardPosition;
            SetPieceAt(piece.BoardPosition, otherPiece);
            SetPieceAt(secondPosition, piece);
            EnablePieceMovement();
        }

        /// <summary>
        /// Checks if the given piece is selected.
        /// </summary>
        /// <param name="piece">The piece to check.</param>
        /// <returns>True if the given piece is selected. False otherwise.</returns>
        public bool IsSelectedPiece(BoardPiece piece) => piece.gameObject.Equals(SelectedPiece.gameObject);

        /// <summary>
        /// Checks if the given piece is the same from other at the given position.
        /// </summary>
        /// <param name="piece">A piece to be check.</param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsSamePiece(BoardPiece piece, Vector2Int position)
        {
            var otherPiece = GetPieceAt(position);
            return otherPiece ? otherPiece.Equals(piece) : false;
        }

        /// <summary>
        /// Checks if the board has a selected piece.
        /// </summary>
        /// <returns>True if the board has selected piece. False otherwise.</returns>
        public bool HasSelectedPiece() => SelectedPiece != null;

        /// <summary>
        /// Checks if a piece exists using the given position.
        /// </summary>
        /// <param name="position">A piece position to check.</param>
        /// <returns>True if a piece exists using the given position. False otherwise.</returns>
        public bool HasPieceAt(Vector2Int position) => GetPieceAt(position) != null;

        public bool CanDropDownPiece(BoardPiece piece)
            => piece && CanDropDownPieceAt(piece.BoardPosition);

        /// <summary>
        /// Checks if a piece can be dropped down using the given position.
        /// </summary>
        /// <param name="position">A piece position to check.</param>
        /// <returns>True if a piece can be dropped down using the given position. False otherwise.</returns>
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
            spriteRenderer.size = GetSizeAsInt();
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
            sounds.PlayPieceSwap();
            yield return SwapPieces(SelectedPiece, piece);

            var matchedPieces = GetMatchedPieces(out bool wasMatch);
            var revertMove = levelSettings.revertIfNoMatch && !wasMatch;
            if (revertMove)
            {
                Sounds.PlayInvalidPieceMove();
                yield return SwapPieces(piece, SelectedPiece);
            }
            UnselectPiece();

            while (wasMatch)
            {
                yield return ComputerMatches(matchedPieces);
                yield return DropDownPieces();
                matchedPieces = GetMatchedPieces(out wasMatch);
            }

            yield return new WaitForSeconds(0.1f);
            EnablePieceMovement();
        }

        private IEnumerator ComputerMatches(List<BoardPiece> matchedPieces)
        {
            DisablePieceMovement();
            var totalScore = 0;
            foreach (var piece in matchedPieces)
            {
                totalScore += piece.GetPoints();
                Sounds.PlayPieceSpawn();
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
            DisablePieceMovement();
            var size = GetSizeAsInt();

            for (int y = 1; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var boardPosition = new Vector2Int(x, y);
                    var currentPiece = GetPieceAt(boardPosition);
                    var cannotDropDown = !CanDropDownPiece(currentPiece);
                    if (cannotDropDown) continue;

                    var droppedBoardPosition = GetDroppedPosition(currentPiece, out int droppedRows);

                    Sounds.PlayDropPieceMove();
                    yield return currentPiece.DropDown(droppedRows);
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
            DisablePieceMovement();
            var size = GetSizeAsInt();
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

                    var currentPiece = PieceFactory.InstantiateRandomPieceWithoutIds(piecesParent, invalidPieceIds);
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

            EnablePieceMovement();
        }

        private List<BoardPiece> GetMatchedPieces(out bool wasMatch)
        {
            var size = GetSizeAsInt();
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
            PieceFactory = new BoardPieceFactory(levelSettings.pieces);

            DisableSelector();
            ResizeSpriteTile();
            RemoveAllPieces();

            StartCoroutine(FillEmptySpots(levelSettings.populateSpawnTime));
        }

        [ContextMenu(POPULATE_BOARD_CONTX_MENU_TITLE, isValidateFunction: true)]
        private bool IsPlayingMode() => Application.isPlaying;
    }
}
