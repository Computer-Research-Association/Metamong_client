using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public sealed class MinesweeperBoard
    {
        private readonly MinesweeperCell[] cells;

        public MinesweeperBoard(MinesweeperBoardDefinition definition)
        {
            if (!definition.IsValid)
            {
                throw new ArgumentException("Board definition is invalid.", nameof(definition));
            }

            Definition = definition;
            cells = new MinesweeperCell[definition.CellCount];

            for (var i = 0; i < cells.Length; i++)
            {
                cells[i] = new MinesweeperCell();
            }
        }

        public MinesweeperBoardDefinition Definition { get; }
        public bool IsGenerated { get; private set; }
        public int FlagsPlaced { get; private set; }
        public int RevealedSafeCellCount { get; private set; }
        public int TotalSafeCellCount => Definition.SafeCellCount;

        public IEnumerable<BoardPosition> AllPositions()
        {
            for (var y = 0; y < Definition.Height; y++)
            {
                for (var x = 0; x < Definition.Width; x++)
                {
                    yield return new BoardPosition(x, y);
                }
            }
        }

        public bool IsWithinBounds(BoardPosition position)
        {
            return position.X >= 0
                && position.X < Definition.Width
                && position.Y >= 0
                && position.Y < Definition.Height;
        }

        public MinesweeperCell GetCell(BoardPosition position)
        {
            if (!IsWithinBounds(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            return cells[IndexOf(position)];
        }

        public bool ToggleFlag(BoardPosition position)
        {
            if (!IsWithinBounds(position))
            {
                return false;
            }

            var cell = GetCell(position);
            if (!cell.ToggleFlag())
            {
                return false;
            }

            FlagsPlaced += cell.IsFlagged ? 1 : -1;
            return true;
        }

        public RevealResult Reveal(BoardPosition position, int? seed = null)
        {
            if (!IsWithinBounds(position))
            {
                return new RevealResult(RevealOutcome.NoChange, Array.Empty<BoardPosition>());
            }

            var cell = GetCell(position);
            if (cell.IsFlagged || cell.IsRevealed)
            {
                return new RevealResult(RevealOutcome.NoChange, Array.Empty<BoardPosition>());
            }

            if (!IsGenerated)
            {
                Generate(position, seed);
                cell = GetCell(position);
            }

            if (cell.HasMine)
            {
                cell.TryReveal();
                return new RevealResult(RevealOutcome.MineTriggered, new[] { position });
            }

            var changedPositions = RevealSafeArea(position);
            var outcome = RevealedSafeCellCount >= TotalSafeCellCount
                ? RevealOutcome.GameWon
                : RevealOutcome.Revealed;

            return new RevealResult(outcome, changedPositions);
        }

        private List<BoardPosition> RevealSafeArea(BoardPosition startPosition)
        {
            var changedPositions = new List<BoardPosition>();
            var frontier = new Queue<BoardPosition>();
            frontier.Enqueue(startPosition);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (!IsWithinBounds(current))
                {
                    continue;
                }

                var cell = GetCell(current);
                if (cell.IsFlagged || cell.IsRevealed || cell.HasMine)
                {
                    continue;
                }

                if (!cell.TryReveal())
                {
                    continue;
                }

                changedPositions.Add(current);
                RevealedSafeCellCount++;

                if (cell.AdjacentMineCount != 0)
                {
                    continue;
                }

                foreach (var neighbor in GetNeighbors(current))
                {
                    var neighborCell = GetCell(neighbor);
                    if (!neighborCell.IsHidden || neighborCell.HasMine)
                    {
                        continue;
                    }

                    frontier.Enqueue(neighbor);
                }
            }

            return changedPositions;
        }

        private void Generate(BoardPosition safePosition, int? seed)
        {
            if (IsGenerated)
            {
                return;
            }

            var random = seed.HasValue ? new Random(seed.Value) : new Random();
            var reservedIndex = IndexOf(safePosition);
            var mineIndices = new HashSet<int>();

            while (mineIndices.Count < Definition.MineCount)
            {
                var candidate = random.Next(cells.Length);
                if (candidate == reservedIndex)
                {
                    continue;
                }

                mineIndices.Add(candidate);
            }

            foreach (var mineIndex in mineIndices)
            {
                cells[mineIndex].SetMine();
            }

            foreach (var position in AllPositions())
            {
                var cell = GetCell(position);
                if (cell.HasMine)
                {
                    continue;
                }

                var adjacentMineCount = 0;
                foreach (var neighbor in GetNeighbors(position))
                {
                    if (GetCell(neighbor).HasMine)
                    {
                        adjacentMineCount++;
                    }
                }

                cell.SetAdjacentMineCount(adjacentMineCount);
            }

            IsGenerated = true;
        }

        private IEnumerable<BoardPosition> GetNeighbors(BoardPosition position)
        {
            for (var y = position.Y - 1; y <= position.Y + 1; y++)
            {
                for (var x = position.X - 1; x <= position.X + 1; x++)
                {
                    if (x == position.X && y == position.Y)
                    {
                        continue;
                    }

                    var neighbor = new BoardPosition(x, y);
                    if (IsWithinBounds(neighbor))
                    {
                        yield return neighbor;
                    }
                }
            }
        }

        private int IndexOf(BoardPosition position)
        {
            return (position.Y * Definition.Width) + position.X;
        }
    }
}
