using UnityEngine;

namespace Minesweeper
{
    [CreateAssetMenu(fileName = "MinesweeperGameConfig", menuName = "Minesweeper/Game Config")]
    public sealed class MinesweeperGameConfig : ScriptableObject
    {
        [SerializeField]
        private MinesweeperBoardDefinition beginner = new(9, 9, 10);

        [SerializeField]
        private MinesweeperBoardDefinition intermediate = new(16, 16, 40);

        [SerializeField]
        private MinesweeperBoardDefinition expert = new(30, 16, 99);

        public bool TryGetDefinition(MinesweeperDifficulty difficulty, out MinesweeperBoardDefinition definition)
        {
            definition = difficulty switch
            {
                MinesweeperDifficulty.Beginner => beginner,
                MinesweeperDifficulty.Intermediate => intermediate,
                MinesweeperDifficulty.Expert => expert,
                _ => beginner
            };

            return definition.IsValid;
        }

        private void OnValidate()
        {
            beginner = Sanitize(beginner, 9, 9, 10);
            intermediate = Sanitize(intermediate, 16, 16, 40);
            expert = Sanitize(expert, 30, 16, 99);
        }

        private static MinesweeperBoardDefinition Sanitize(
            MinesweeperBoardDefinition definition,
            int defaultWidth,
            int defaultHeight,
            int defaultMineCount)
        {
            var width = Mathf.Max(2, definition.Width == 0 ? defaultWidth : definition.Width);
            var height = Mathf.Max(2, definition.Height == 0 ? defaultHeight : definition.Height);
            var maxMineCount = (width * height) - 1;
            var fallbackMineCount = Mathf.Min(defaultMineCount, maxMineCount);
            var mineCount = definition.MineCount == 0 ? fallbackMineCount : definition.MineCount;
            mineCount = Mathf.Clamp(mineCount, 1, maxMineCount);

            return new MinesweeperBoardDefinition(width, height, mineCount);
        }
    }
}
