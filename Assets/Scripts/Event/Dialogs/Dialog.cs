#nullable enable
using UnityEngine;
using Assets.Scripts.UI.Game;

namespace Assets.Scripts.Event.Dialogs
{
    public class Dialog : IDialog
    {
        public bool ShowDialogMainWindow { get; }
        public bool UseOption { get; }
        public Sprite? Image { get; }
        public (string, string) OptionTexts { get; }
        public (string, string) OptionJumpLabel { get; }
        public int SpriteIndex { get; }
        public string Name { get; }
        public string Message { get; }
        public int MessageSpeed { get; }
        public DialogUi.TextEffect TextEffect { get; }

        public Dialog(
            bool showDialogMainWindow = true,
            Sprite? image = null,
            (string, string)? optionTexts = null,
            (string, string)? optionJumpLabel = null,
            int spriteIndex = -1,
            string? name = null,
            string? message = null,
            int messageSpeed = 2000,
            DialogUi.TextEffect textEffect = DialogUi.TextEffect.None)
        {
            ShowDialogMainWindow = showDialogMainWindow;
            Image = image;
            OptionTexts = optionTexts ?? (string.Empty, string.Empty);
            OptionJumpLabel = optionJumpLabel ?? (string.Empty, string.Empty);
            SpriteIndex = spriteIndex;
            Name = name ?? string.Empty;
            Message = message ?? string.Empty;
            MessageSpeed = messageSpeed;
            TextEffect = textEffect;
            UseOption = optionJumpLabel != null;
        }
    }
}
