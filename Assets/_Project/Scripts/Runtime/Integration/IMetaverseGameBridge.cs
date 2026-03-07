namespace Minesweeper
{
    public interface IMetaverseGameBridge
    {
        void NotifyGameStarted(MinesweeperDifficulty difficulty, MinesweeperBoardDefinition definition);
        void NotifyFlagCountChanged(int flagsPlaced, int totalMineCount);
        void NotifyGameFinished(MinesweeperSessionResult result);
    }
}
