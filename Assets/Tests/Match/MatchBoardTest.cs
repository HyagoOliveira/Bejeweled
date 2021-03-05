using UnityEngine;
using NUnit.Framework;
using Bejeweled.Macth;

namespace Bejeweled.Tests.Macth
{
    public class BoardTest
    {
        private Board board;
        private BoardSettings levelSettings;

        [SetUp]
        public void Setup()
        {
            CreateBoardSettings();
            InstanciateBoardPrefab();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(levelSettings);
            Object.Destroy(board.gameObject);
        }

        [Test]
        public void WhenResized_BoardSize_ShouldBeTheSameFromItsSettings()
        {
            var actualSize = board.GetSize();
            var expectedSize = levelSettings.size;

            Assert.AreEqual(expectedSize, actualSize);
        }

        [Test]
        public void WhenPopulated_BoardPieceCount_ShouldHasTheSameCountFromItsSettings()
        {
            var actualPiecesCount = board.PieceManager.PiecesCount;
            var expectedPiecesCount = levelSettings.PiecesCount;

            Assert.AreEqual(expectedPiecesCount, actualPiecesCount);
        }

        [Test]
        public void WhenPopulated_AllBoardPieces_ShouldNotBeOnInitialMatch()
        {
            var size = board.GetSize();

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var currentPieceId = board.GetPieceIdAt(x, y);

                    var closestRightPieceId = board.GetPieceIdAt(x + 1, y);
                    var furtherRightPieceId = board.GetPieceIdAt(x + 2, y);

                    var closestTopPieceId = board.GetPieceIdAt(x, y + 1);
                    var furtherTopPieceId = board.GetPieceIdAt(x, y + 2);

                    var isVertMatch = currentPieceId == closestTopPieceId && currentPieceId == furtherTopPieceId;
                    var isHortMatch = currentPieceId == closestRightPieceId && currentPieceId == furtherRightPieceId;

                    Assert.IsFalse(isVertMatch, "A vertical match was detect when populating a board.");
                    Assert.IsFalse(isHortMatch, "A horizontal match was detect when populating a board.");
                }
            }
        }

        [Test]
        public void WhenPopulated_IsAdjacentPositionFunction_ShouldAlwaysBeCorrect()
        {
            var halfSize = board.GetSize() / 2;
            var centerPosition = halfSize;

            var closestRightPosition = centerPosition + Vector2Int.right;
            var furtherRightPosition = centerPosition + Vector2Int.right * 2;

            var closestTopPosition = centerPosition + Vector2Int.up;
            var furtherTopPosition = centerPosition + Vector2Int.up * 2;

            var closestTopRightPosition = centerPosition + Vector2Int.one;
            var furtherTopRightPosition = centerPosition + Vector2Int.one * 2;

            void TestNonAdjacentPositions(Vector2Int wrongPosition)
            {
                var shouldNotBeAdjacent = board.IsAdjacentPosition(centerPosition, wrongPosition);
                Assert.IsFalse(shouldNotBeAdjacent, $"Piece position {centerPosition} should not be adjacent to {wrongPosition}");
            }

            void TestAdjacentPositions(Vector2Int correctPosition, Vector2Int wrongPosition)
            {
                var shouldBeAdjacent = board.IsAdjacentPosition(centerPosition, correctPosition);

                Assert.IsTrue(shouldBeAdjacent, $"Piece position {centerPosition} should be adjacent to {correctPosition}");
                TestNonAdjacentPositions(wrongPosition);
            }

            TestAdjacentPositions(closestTopPosition, furtherTopPosition);
            TestAdjacentPositions(closestRightPosition, furtherRightPosition);

            TestNonAdjacentPositions(closestTopRightPosition);
            TestNonAdjacentPositions(furtherTopRightPosition);
        }

        private void CreateBoardSettings()
        {
            levelSettings = ScriptableObject.CreateInstance<BoardSettings>();
            levelSettings.size = Vector2Int.one * 8;
            levelSettings.pieces = new GameObject[]
            {
                TestUtility.FindPrefab("BlueGem"),
                TestUtility.FindPrefab("GreenGem"),
                TestUtility.FindPrefab("VioletGem"),
                TestUtility.FindPrefab("RedGem"),
                TestUtility.FindPrefab("YellowGem")
            };
        }

        private void InstanciateBoardPrefab()
        {
            const string prefabName = "MatchBoard";
            var boardPrefab = TestUtility.FindPrefab(prefabName);
            board = Object.Instantiate(boardPrefab).GetComponent<Board>();
        }
    }
}
