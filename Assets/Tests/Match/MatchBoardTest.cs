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
        public void Board_WhenCreated_HasSamePieceCountFromLevelSettings()
        {
            var actualPiecesCount = board.PieceManager.PiecesCount;
            var expectedPiecesCount = levelSettings.PiecesCount;

            Assert.AreEqual(expectedPiecesCount, actualPiecesCount);
        }

        private void CreateLevelSettings()
        {
            levelSettings = ScriptableObject.CreateInstance<MatchLevelSettings>();
        }

        private void InstanciateBoardPrefab()
        {
            const string prefabName = "MatchBoard";
            var boardPrefab = TestUtility.FindPrefab(prefabName);
            board = Object.Instantiate(boardPrefab).GetComponent<MatchBoard>();
        }
    }
}
