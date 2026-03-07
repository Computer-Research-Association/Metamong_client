using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public sealed class MinesweeperBoardView : MonoBehaviour
    {
        [SerializeField]
        private MinesweeperGameController gameController;

        [SerializeField]
        private MinesweeperCellView cellPrefab;

        [SerializeField]
        private GridLayoutGroup gridLayoutGroup;

        private readonly Dictionary<BoardPosition, MinesweeperCellView> spawnedCells = new();

        private RectTransform rectTransform;

        private void Awake()
        {
            ResolveReferences();
        }

        private void Reset()
        {
            ResolveReferences();
        }

        private void OnValidate()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            if (gameController == null)
            {
                return;
            }

            gameController.BoardReset += HandleBoardReset;
            gameController.BoardChanged += HandleBoardChanged;
            gameController.GameFinished += HandleGameFinished;

            if (gameController.Board != null)
            {
                RebuildBoard();
            }
        }

        private void OnDisable()
        {
            if (gameController == null)
            {
                return;
            }

            gameController.BoardReset -= HandleBoardReset;
            gameController.BoardChanged -= HandleBoardChanged;
            gameController.GameFinished -= HandleGameFinished;
        }

        private void HandleBoardReset()
        {
            RebuildBoard();
        }

        private void HandleBoardChanged(IReadOnlyList<BoardPosition> changedPositions)
        {
            if (gameController?.Board == null)
            {
                return;
            }

            foreach (var position in changedPositions)
            {
                RefreshCell(position);
            }
        }

        private void HandleGameFinished(MinesweeperSessionResult _)
        {
            RefreshAllCells();
        }

        private void RebuildBoard()
        {
            if (gameController?.Board == null)
            {
                return;
            }

            if (cellPrefab == null)
            {
                Debug.LogError("MinesweeperBoardView needs a Cell Prefab reference.");
                return;
            }

            ResolveReferences();
            ClearSpawnedCells();
            ApplyBoardLayout(gameController.Board.Definition);

            foreach (var position in gameController.Board.AllPositions())
            {
                var cellView = Instantiate(cellPrefab, transform);
                cellView.Initialize(position, HandleRevealRequested, HandleFlagRequested);
                cellView.transform.localScale = Vector3.one;
                spawnedCells[position] = cellView;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            RefreshAllCells();
        }

        private void RefreshAllCells()
        {
            if (gameController?.Board == null)
            {
                return;
            }

            foreach (var position in gameController.Board.AllPositions())
            {
                RefreshCell(position);
            }
        }

        private void RefreshCell(BoardPosition position)
        {
            if (gameController?.Board == null)
            {
                return;
            }

            if (!spawnedCells.TryGetValue(position, out var cellView))
            {
                return;
            }

            var cell = gameController.Board.GetCell(position);
            cellView.Refresh(cell, gameController.Phase);
        }

        private void HandleRevealRequested(BoardPosition position)
        {
            gameController?.RevealCell(position.X, position.Y);
        }

        private void HandleFlagRequested(BoardPosition position)
        {
            gameController?.ToggleFlag(position.X, position.Y);
        }

        private void ApplyBoardLayout(MinesweeperBoardDefinition definition)
        {
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = definition.Width;

            var availableWidth = rectTransform.rect.width
                - gridLayoutGroup.padding.left
                - gridLayoutGroup.padding.right
                - (gridLayoutGroup.spacing.x * (definition.Width - 1));
            var availableHeight = rectTransform.rect.height
                - gridLayoutGroup.padding.top
                - gridLayoutGroup.padding.bottom
                - (gridLayoutGroup.spacing.y * (definition.Height - 1));

            var cellWidth = availableWidth / definition.Width;
            var cellHeight = availableHeight / definition.Height;
            var cellSize = Mathf.Floor(Mathf.Min(cellWidth, cellHeight));

            if (cellSize > 0f)
            {
                gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
            }
        }

        private void ClearSpawnedCells()
        {
            spawnedCells.Clear();

            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void ResolveReferences()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (gridLayoutGroup == null)
            {
                gridLayoutGroup = GetComponent<GridLayoutGroup>();
            }
        }
    }
}
