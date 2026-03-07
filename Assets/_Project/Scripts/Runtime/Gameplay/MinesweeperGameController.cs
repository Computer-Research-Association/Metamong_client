using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper
{
    public sealed class MinesweeperGameController : MonoBehaviour
    {
        [SerializeField]
        private MinesweeperGameConfig gameConfig;

        [SerializeField]
        private MinesweeperDifficulty startingDifficulty = MinesweeperDifficulty.Beginner;

        [SerializeField]
        private bool autoStartOnStart = true;

        public event Action BoardReset;
        public event Action<IReadOnlyList<BoardPosition>> BoardChanged;
        public event Action<int, int> FlagCountChanged;
        public event Action<MinesweeperSessionResult> GameFinished;

        public IMetaverseGameBridge Bridge { private get; set; }
        public MinesweeperBoard Board { get; private set; }
        public MinesweeperDifficulty CurrentDifficulty { get; private set; }
        public GamePhase Phase { get; private set; } = GamePhase.NotReady;
        public float ElapsedSeconds { get; private set; }

        private void Start()
        {
            if (autoStartOnStart)
            {
                ResetGame(startingDifficulty);
            }
        }

        private void Update()
        {
            if (Phase == GamePhase.Playing)
            {
                ElapsedSeconds += Time.deltaTime;
            }
        }

        public void ResetGame(MinesweeperDifficulty difficulty)
        {
            if (gameConfig == null)
            {
                Debug.LogError("MinesweeperGameController needs a MinesweeperGameConfig asset.");
                Phase = GamePhase.NotReady;
                return;
            }

            if (!gameConfig.TryGetDefinition(difficulty, out var definition))
            {
                Debug.LogError($"No valid board definition found for difficulty {difficulty}.");
                Phase = GamePhase.NotReady;
                return;
            }

            Board = new MinesweeperBoard(definition);
            CurrentDifficulty = difficulty;
            ElapsedSeconds = 0f;
            Phase = GamePhase.Playing;

            BoardReset?.Invoke();
            FlagCountChanged?.Invoke(Board.FlagsPlaced, Board.Definition.MineCount);
            Bridge?.NotifyGameStarted(CurrentDifficulty, definition);
        }

        public RevealOutcome RevealCell(int x, int y)
        {
            if (Board == null || Phase != GamePhase.Playing)
            {
                return RevealOutcome.NoChange;
            }

            var result = Board.Reveal(new BoardPosition(x, y));
            if (result.ChangedPositions.Count > 0)
            {
                BoardChanged?.Invoke(result.ChangedPositions);
            }

            if (result.Outcome == RevealOutcome.MineTriggered)
            {
                FinishGame(cleared: false);
            }
            else if (result.Outcome == RevealOutcome.GameWon)
            {
                FinishGame(cleared: true);
            }

            return result.Outcome;
        }

        public bool ToggleFlag(int x, int y)
        {
            if (Board == null || Phase != GamePhase.Playing)
            {
                return false;
            }

            var position = new BoardPosition(x, y);
            if (!Board.ToggleFlag(position))
            {
                return false;
            }

            BoardChanged?.Invoke(new[] { position });
            FlagCountChanged?.Invoke(Board.FlagsPlaced, Board.Definition.MineCount);
            Bridge?.NotifyFlagCountChanged(Board.FlagsPlaced, Board.Definition.MineCount);
            return true;
        }

        private void FinishGame(bool cleared)
        {
            if (Board == null || Phase != GamePhase.Playing)
            {
                return;
            }

            Phase = cleared ? GamePhase.Won : GamePhase.Lost;

            var result = new MinesweeperSessionResult(
                CurrentDifficulty,
                cleared,
                ElapsedSeconds,
                Board.RevealedSafeCellCount,
                Board.TotalSafeCellCount,
                Board.Definition.MineCount,
                Board.FlagsPlaced);

            GameFinished?.Invoke(result);
            Bridge?.NotifyGameFinished(result);
        }
    }
}
