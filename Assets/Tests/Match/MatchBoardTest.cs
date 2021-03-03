using UnityEngine;
using NUnit.Framework;
using Bejeweled.Macth;

namespace Bejeweled.Tests.Macth
{
    public class MatchBoardTest
    {
        private MatchBoard board;
        private MatchLevelSettings levelSettings;

        [SetUp]
        public void Setup()
        {
            CreateLevelSettings();
            InstanciateBoardPrefab();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(levelSettings);
            Object.Destroy(board.gameObject);
        }

        [Test]
        public void Board_WhenCreated_HasSameSizeFromLevelSettings()
        {
            var actualSize = board.GetSize();
            var expectedSize = levelSettings.boardSize;

            Assert.AreEqual(expectedSize, actualSize);
        }

        [Test]
        public void Board_WhenPopulated_HasNoInitialMatch()
        {
            var size = board.GetSize();

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var currentPieceId = board.GetPieceIdAt(x, y);

                    var rightmostPieceId = board.GetPieceIdAt(x + 1, y);
                    var rightfarPieceId = board.GetPieceIdAt(x + 2, y);

                    var topmostPieceId = board.GetPieceIdAt(x, y + 1);
                    var topfarPieceId = board.GetPieceIdAt(x, y + 2);

                    var isVertMatch = currentPieceId == topmostPieceId && currentPieceId == topfarPieceId;
                    var isHortMatch = currentPieceId == rightmostPieceId && currentPieceId == rightfarPieceId;

                    Assert.IsFalse(isVertMatch, "A vertical match was detect when populating a board.");
                    Assert.IsFalse(isHortMatch, "A horizontal match was detect when populating a board.");
                }
            }
        }

        [Test]
        public void Board_Check_AdjacentPieces()
        {
            var halfSize = board.GetSize() / 2;
            var centerPiecePosition = halfSize;
            var rightmostPiecePosition = centerPiecePosition + Vector2Int.right;
            var rightfarPiecePosition = centerPiecePosition + Vector2Int.right * 2;
            var topmostPiecePosition = centerPiecePosition + Vector2Int.up;
            var topfarPiecePosition = centerPiecePosition + Vector2Int.up * 2;
            var toprightmostPosition = centerPiecePosition + Vector2Int.one;
            var toprightfarPosition = centerPiecePosition + Vector2Int.one * 2;

            var shouldBeAdjacent = board.IsAdjacentPosition(centerPiecePosition, rightmostPiecePosition);
            var shouldNotBeAdjacent = board.IsAdjacentPosition(centerPiecePosition, rightfarPiecePosition);

            Assert.IsTrue(shouldBeAdjacent, $"Piece position {centerPiecePosition} should be adjacent to {rightmostPiecePosition}");
            Assert.IsFalse(shouldNotBeAdjacent, $"Piece position {centerPiecePosition} should not be adjacent to {rightfarPiecePosition}");

            shouldBeAdjacent = board.IsAdjacentPosition(centerPiecePosition, topmostPiecePosition);
            shouldNotBeAdjacent = board.IsAdjacentPosition(centerPiecePosition, topfarPiecePosition);

            Assert.IsTrue(shouldBeAdjacent, $"Piece position {centerPiecePosition} should be adjacent to {topmostPiecePosition}");
            Assert.IsFalse(shouldNotBeAdjacent, $"Piece position {centerPiecePosition} should not be adjacent to {topfarPiecePosition}");

            shouldNotBeAdjacent = board.IsAdjacentPosition(centerPiecePosition, toprightmostPosition);
            Assert.IsFalse(shouldNotBeAdjacent, $"Piece position {centerPiecePosition} should not be adjacent to {toprightmostPosition}");

            shouldNotBeAdjacent = board.IsAdjacentPosition(centerPiecePosition, toprightfarPosition);
            Assert.IsFalse(shouldNotBeAdjacent, $"Piece position {centerPiecePosition} should not be adjacent to {toprightfarPosition}");
        }

        [Test]
        public void Board_WhenCreated_HasSamePieceCountFromLevelSettings()
        {
            var actualPiecesCount = board.PieceManager.PiecesCount;
            var expectedPiecesCount = levelSettings.PiecesCount;

            Assert.AreEqual(expectedPiecesCount, actualPiecesCount);
        }

        private void CreateLevelSettings()
        {
            levelSettings = ScriptableObject.CreateInstance<MatchLevelSettings>();
            levelSettings.boardSize = Vector2Int.one * 8;
            levelSettings.pieces = new GameObject[]
            {
                TestUtility.FindPrefab("BlueGem"),
                TestUtility.FindPrefab("GreenGem"),
                TestUtility.FindPrefab("OrangeGem"),
                TestUtility.FindPrefab("RedGem"),
                TestUtility.FindPrefab("YellowGem")
            };
        }

        private void InstanciateBoardPrefab()
        {
            const string prefabName = "MatchBoard";
            var boardPrefab = TestUtility.FindPrefab(prefabName);
            board = Object.Instantiate(boardPrefab).GetComponent<MatchBoard>();
        }
    }
}
