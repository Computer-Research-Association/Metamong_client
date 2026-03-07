using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Minesweeper
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MinesweeperCellView : MonoBehaviour, IPointerClickHandler
    {
        [Header("References")]
        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private TMP_Text label;

        [SerializeField]
        private Button button;

        [Header("Colors")]
        [SerializeField]
        private Color hiddenColor = new(0.75f, 0.75f, 0.75f, 1f);

        [SerializeField]
        private Color revealedColor = new(0.92f, 0.92f, 0.92f, 1f);

        [SerializeField]
        private Color flaggedColor = new(0.95f, 0.80f, 0.35f, 1f);

        [SerializeField]
        private Color mineColor = new(0.95f, 0.35f, 0.35f, 1f);

        private BoardPosition boardPosition;
        private Action<BoardPosition> revealRequested;
        private Action<BoardPosition> flagRequested;

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

        public void Initialize(
            BoardPosition position,
            Action<BoardPosition> onRevealRequested,
            Action<BoardPosition> onFlagRequested)
        {
            boardPosition = position;
            revealRequested = onRevealRequested;
            flagRequested = onFlagRequested;
            gameObject.name = $"Cell_{position.X}_{position.Y}";
        }

        public void Refresh(MinesweeperCell cell, GamePhase phase)
        {
            ResolveReferences();

            var showMine = phase == GamePhase.Lost && cell.HasMine;
            var isRevealed = cell.IsRevealed || showMine;

            if (backgroundImage != null)
            {
                backgroundImage.color = cell.IsFlagged
                    ? flaggedColor
                    : showMine
                        ? mineColor
                        : isRevealed
                            ? revealedColor
                            : hiddenColor;
            }

            if (label != null)
            {
                if (cell.IsFlagged)
                {
                    label.text = "F";
                }
                else if (showMine)
                {
                    label.text = "*";
                }
                else if (!isRevealed)
                {
                    label.text = string.Empty;
                }
                else if (cell.AdjacentMineCount <= 0)
                {
                    label.text = string.Empty;
                }
                else
                {
                    label.text = cell.AdjacentMineCount.ToString();
                }

                label.color = GetLabelColor(cell, showMine);
            }

            if (button != null)
            {
                button.interactable = phase == GamePhase.Playing && !cell.IsRevealed;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                revealRequested?.Invoke(boardPosition);
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                flagRequested?.Invoke(boardPosition);
            }
        }

        private Color GetLabelColor(MinesweeperCell cell, bool showMine)
        {
            if (showMine)
            {
                return Color.white;
            }

            return cell.AdjacentMineCount switch
            {
                1 => new Color(0.15f, 0.35f, 0.95f, 1f),
                2 => new Color(0.15f, 0.60f, 0.20f, 1f),
                3 => new Color(0.85f, 0.20f, 0.20f, 1f),
                4 => new Color(0.35f, 0.20f, 0.70f, 1f),
                5 => new Color(0.60f, 0.20f, 0.20f, 1f),
                6 => new Color(0.10f, 0.55f, 0.55f, 1f),
                7 => new Color(0.20f, 0.20f, 0.20f, 1f),
                8 => new Color(0.45f, 0.45f, 0.45f, 1f),
                _ => Color.black
            };
        }

        private void ResolveReferences()
        {
            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>(true);
            }
        }
    }
}
