#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Assets.Scripts.Event.Dialogs;
using Assets.Scripts.UI.Game;

namespace Assets.Scripts.Event
{
    public class DialogParser
    {
        private readonly Dictionary<string, List<IDialog>> _cachedDialogs;

        public DialogParser()
        {
            _cachedDialogs = new Dictionary<string, List<IDialog>>();
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
        }

        ~DialogParser() => LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;

        private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj) => _cachedDialogs.Clear();

        public bool TryGetCachedDialogs(string dialogId, out List<IDialog> value) => _cachedDialogs.TryGetValue(dialogId, out value);

        public List<IDialog> Parse(string dialogId, string dialog, List<Event.ImageInfo> imageInfos)
        {
            var dialogs = dialog.Split('\n');
            for (var index = 0; index < dialogs.Length; index++) dialogs[index] = dialogs[index].Trim();
            
            var parsedDialogs = new List<IDialog>();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < dialogs.Length; index++)
            {
                var strings = dialogs[index].Split('|');

                if (int.TryParse(strings[0], out var spriteIndex))
                {
                    Sprite? image = null;
                    (string, string)? optionTexts = null;
                    (string, string)? optionLabels = null;
                    var name = strings[1] == "-" ? string.Empty : strings[1];
                    var message = strings[4] == "-" ? string.Empty : strings[4];
                    var showDialogMainWindow = !(name == string.Empty && message == string.Empty && spriteIndex == -1);
                    
                    if (5 < strings.Length)
                    {
                        switch (strings[5])
                        {
                            case "P":
                                // ReSharper disable once ForCanBeConvertedToForeach
                                for (var i = 0; i < imageInfos.Count; i++)
                                {
                                    var item = imageInfos[i];
                                    if (item.ImageId != strings[6]) continue;
                                    image = item.Sprite;
                                    break;
                                }

                                break;

                            case "J":
                                optionTexts = (strings[6], strings[7]);
                                optionLabels = (strings[8], strings[9]);
                                break;

                            case "PJ":
                                // ReSharper disable once ForCanBeConvertedToForeach
                                for (var i = 0; i < imageInfos.Count; i++)
                                {
                                    var item = imageInfos[i];
                                    if (item.ImageId != strings[6]) continue;
                                    image = item.Sprite;

                                    optionTexts = (strings[7], strings[8]);
                                    optionLabels = (strings[9], strings[10]);
                                    break;
                                }

                                break;
                        }
                    }

                    parsedDialogs.Add(new Dialog(
                        showDialogMainWindow,
                        image,
                        optionTexts,
                        optionLabels,
                        spriteIndex,
                        name,
                        message,
                        int.Parse(strings[2]),
                        strings[3] == "w" ? DialogUi.TextEffect.Wiggle : DialogUi.TextEffect.None
                    ));
                }
                else
                {
                    switch (strings[0])
                    {
                        case "L":
                            parsedDialogs.Add(new Label(strings[1]));
                            break;
                        case "E":
                            parsedDialogs.Add(new Exit(int.Parse(strings[1])));
                            break;
                    }
                }
            }
            _cachedDialogs.Add(dialogId, parsedDialogs);
            return parsedDialogs;
        }
    }
}
