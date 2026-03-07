using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper
{
    public enum MinesweeperDifficulty
    {
        Beginner,
        Intermediate,
        Expert
    }

    public enum CellVisibility
    {
        Hidden,
        Flagged,
        Revealed
    }

    public enum RevealOutcome
    {
        NoChange,
        Revealed,
        MineTriggered,
        GameWon
    }

    public enum GamePhase
    {
        NotReady,
        Playing,
        Won,
        Lost
    }

    [Serializable]
    public struct MinesweeperBoardDefinition
    {
        [Min(2)]
        public int Width;

        [Min(2)]
        public int Height;

        [Min(1)]
        public int MineCount;

        public MinesweeperBoardDefinition(int width, int height, int mineCount)
        {
            Width = width;
            Height = height;
            MineCount = mineCount;
        }

        public int CellCount => Width * Height;
        public int SafeCellCount => CellCount - MineCount;
        public bool IsValid => Width >= 2 && Height >= 2 && MineCount > 0 && MineCount < CellCount;
    }

    public readonly struct BoardPosition : IEquatable<BoardPosition>
    {
        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals(BoardPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public sealed class MinesweeperCell
    {
        public bool HasMine { get; private set; }
        public int AdjacentMineCount { get; private set; }
        public CellVisibility Visibility { get; private set; } = CellVisibility.Hidden;

        public bool IsHidden => Visibility == CellVisibility.Hidden;
        public bool IsFlagged => Visibility == CellVisibility.Flagged;
        public bool IsRevealed => Visibility == CellVisibility.Revealed;

        internal void SetMine()
        {
            HasMine = true;
        }

        internal void SetAdjacentMineCount(int count)
        {
            AdjacentMineCount = Math.Max(0, count);
        }

        public bool TryReveal()
        {
            if (!IsHidden)
            {
                return false;
            }

            Visibility = CellVisibility.Revealed;
            return true;
        }

        public bool ToggleFlag()
        {
            if (IsRevealed)
            {
                return false;
            }

            Visibility = IsFlagged ? CellVisibility.Hidden : CellVisibility.Flagged;
            return true;
        }
    }

    public readonly struct RevealResult
    {
        public RevealResult(RevealOutcome outcome, IReadOnlyList<BoardPosition> changedPositions)
        {
            Outcome = outcome;
            ChangedPositions = changedPositions ?? Array.Empty<BoardPosition>();
        }

        public RevealOutcome Outcome { get; }
        public IReadOnlyList<BoardPosition> ChangedPositions { get; }
    }

    public readonly struct MinesweeperSessionResult
    {
        public MinesweeperSessionResult(
            MinesweeperDifficulty difficulty,
            bool cleared,
            float elapsedSeconds,
            int revealedSafeCells,
            int totalSafeCells,
            int mineCount,
            int flagsPlaced)
        {
            Difficulty = difficulty;
            Cleared = cleared;
            ElapsedSeconds = elapsedSeconds;
            RevealedSafeCells = revealedSafeCells;
            TotalSafeCells = totalSafeCells;
            MineCount = mineCount;
            FlagsPlaced = flagsPlaced;
        }

        public MinesweeperDifficulty Difficulty { get; }
        public bool Cleared { get; }
        public float ElapsedSeconds { get; }
        public int RevealedSafeCells { get; }
        public int TotalSafeCells { get; }
        public int MineCount { get; }
        public int FlagsPlaced { get; }
    }
}
