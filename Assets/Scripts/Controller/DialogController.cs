#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Assets.Scripts.Event;
using Assets.Scripts.UI.Game;
using Assets.Scripts.Event.Dialogs;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DialogUi))]
    public class DialogController : MonoBehaviour
    {
        [SerializeField]
#pragma warning disable 649
        // ReSharper disable once CollectionNeverUpdated.Local
        private List<Sprite>? _sprites;
#pragma warning restore 649
        private DialogUi? _dialogUi;
        private List<IDialog>? _dialogs;
        private int _currentIndex;
        private bool _dialogOptioning;
        private static DialogParser? _dialogParser;
        private Action<int>? _onComplete;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _dialogUi = GetComponent<DialogUi>();
            _dialogParser ??= new DialogParser();
            _dialogUi!.ClickedEvent.AddListener(OnDialogClicked);
            GameManager.Ui!.OnDialogButtonClicked += OnDialogClicked;
            EventManager.Dialog = this;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            EventManager.Dialog = null;
            _dialogUi!.ClickedEvent.RemoveListener(OnDialogClicked);
            if (GameManager.Ui != null) 
                GameManager.Ui!.OnDialogButtonClicked -= OnDialogClicked;
        }

        private void OnDialogClicked()
        {
            if (_dialogUi!.ShowingText)
                _dialogUi.SkipText();
            else if (_dialogs != null)
            {
                if (_currentIndex < _dialogs.Count)
                {
                    if (!_dialogOptioning)
                        ProcessDialog();
                }
                else
                {
                    _dialogUi!.StopWiggle();
                    _dialogUi.HideUi();
                    _dialogUi.HideBranch();
                    _dialogUi.HideCenterImage();
                    _onComplete?.Invoke(0);
                    _onComplete = null;
                }
            }
            else
            {
                _dialogUi!.StopWiggle();
                _dialogUi.HideUi();
                _dialogUi.HideBranch();
                _dialogUi.HideCenterImage();
                _onComplete?.Invoke(0);
                _onComplete = null;
            }
        }

        public void PlayText(string eventId, List<Event.Event.ImageInfo> imageInfos, Action<int>? onComplete = null)
        {
            var splited = eventId.Split('/');
            var asyncOperationHandle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(splited[0], splited[1]);
            if (asyncOperationHandle.IsDone)
                PlayText(_dialogParser!.TryGetCachedDialogs(eventId, out var value)
                    ? value
                    : _dialogParser.Parse(eventId, asyncOperationHandle.Result, imageInfos), onComplete);
            else
                asyncOperationHandle.Completed += handle =>
                    PlayText(_dialogParser!.TryGetCachedDialogs(eventId, out var value)
                        ? value
                        : _dialogParser.Parse(eventId, handle.Result, imageInfos), onComplete);
        }

        public void PlayText(List<IDialog> dialogs, Action<int>? onComplete = null)
        {
            if (dialogs.Count == 0)
                return;
            _dialogs = dialogs;
            _currentIndex = 0;
            _onComplete = onComplete;
            ProcessDialog();
        }

        private void ProcessDialog()
        {
            var currentDialog = _dialogs![_currentIndex];

            switch (currentDialog)
            {
                case Dialog dialog:
                    if (!dialog.ShowDialogMainWindow)
                    {
                        _dialogUi!.StopWiggle();
                        _dialogUi.HideUi();
                    }
                    else
                    {
                        if (dialog.SpriteIndex == -1)
                            _dialogUi!.HideCharactorImage();
                        else
                        {
                            _dialogUi!.ShowCharactorImage();
                            if (!ReferenceEquals(_dialogUi.DisplaySprite, _sprites![dialog.SpriteIndex]))
                                _dialogUi.DisplaySprite = _sprites[dialog.SpriteIndex];
                        }

                        if (dialog.Name == string.Empty)
                            _dialogUi.HideName();
                        else
                        {
                            _dialogUi.ShowName();
                            if (_dialogUi.DisplayName != dialog.Name)
                                _dialogUi.DisplayName = dialog.Name;
                        }

                        _dialogUi.PlayText(
                            dialog.Message,
                            dialog.MessageSpeed,
                            () =>
                            {
                                if (!dialog.UseOption) _currentIndex += 1;
                            },
                            dialog.TextEffect);

                        _dialogUi!.ShowUi();
                    }

                    if (dialog.Image != null)
                    {
                        _dialogUi.ShowCenterImage();
                        if (!ReferenceEquals(_dialogUi.DisplaySprite, dialog.Image))
                            _dialogUi.DisplayCenterSprite = dialog.Image;
                    }
                    else
                        _dialogUi.HideCenterImage();

                    if (dialog.UseOption)
                    {
                        _dialogOptioning = true;
                        _dialogUi.UpdateBranch(dialog.OptionTexts.Item1, dialog.OptionTexts.Item2,
                            selectedOption =>
                            {
                                _currentIndex = selectedOption switch
                                {
                                    0 => _dialogs.FindIndex(item =>
                                        item is Label label && label.LabelName == dialog.OptionJumpLabel.Item1),
                                    1 => _dialogs.FindIndex(item =>
                                        item is Label label && label.LabelName == dialog.OptionJumpLabel.Item2),
                                    _ => throw new Exception()
                                };
                                _dialogOptioning = false;
                                ProcessDialog();
                            });
                        _dialogUi.ShowBranch();
                    }
                    else
                        _dialogUi.HideBranch();

                    break;
                    
                case Label _:
                    var nextindex = _currentIndex + 1;
                    while (_dialogs![nextindex] is Label)
                        nextindex += 1;
                    
                    _currentIndex = nextindex;
                    // ReSharper disable once TailRecursiveCall
                    ProcessDialog();
                    break;
                case Exit exit:
                    _dialogUi!.StopWiggle();
                    _dialogUi.HideUi();
                    _dialogUi.HideBranch();
                    _dialogUi.HideCenterImage();
                    _onComplete?.Invoke(exit.ExitValue);
                    _onComplete = null;
                    break;
            }
        }
    }
}
