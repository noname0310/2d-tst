#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.UI.Core;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    public class SceneLoader : MonoBehaviour
    {
        public static event Action? OnStageChange;
        public static event Action? OnStageChanged;
        public static event Action? OnStageLoad;
        public static event Action? OnStageLoaded;
        public static event Action? OnStageInitalize;
        public static bool IsStageChanging;
        public static bool IsStageLoading;
        public LoadUi? LoadUi;
        //todo seemless audio transition between scenes
        //public AudioListener? AudioListener;
        private static SceneLoader? _sceneLoader;
        private int _leftLoadSceneCount;
        private SceneType _currentLoadedScene;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _sceneLoader = this;
            _currentLoadedScene = SceneType.None;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy() => _sceneLoader = null;

        public static void LoadTitle() =>
            _sceneLoader!.LoadUi!.Show(LoadUi.LoadingType.PrograssBar, () =>
                _sceneLoader.StartCoroutine(_sceneLoader.LoadTitleAsync()));

        public static void LoadGame()
        {
            _sceneLoader!.LoadUi!.Show(LoadUi.LoadingType.PrograssBar, () =>
                _sceneLoader.StartCoroutine(_sceneLoader.LoadGameAsync()));
        }

        public static void LoadCredit()
        {
            _sceneLoader!.LoadUi!.Show(LoadUi.LoadingType.CenterShadow, () =>
                _sceneLoader.StartCoroutine(_sceneLoader.LoadCreditAsync()));
        }

        public static void ChangeStage(string stageName, ShadowTransitionDirection shadowTransitionDirection) =>
            _sceneLoader!.StartCoroutine(_sceneLoader.ChangeStageAsync(stageName, shadowTransitionDirection));

        private IEnumerator LoadTitleAsync()
        {
            yield return UnloadEntireCurrentSceneAsync();
            SceneManager.sceneLoaded += LoadSceneEnd;
            _leftLoadSceneCount = 1;
            yield return LoadPrograssSingleSceneAsync("Title", "Scenes/TitleScene/Title");
            GameManager.Sound!.StopGlobalSound();
            _currentLoadedScene = SceneType.Title;
        }

        private IEnumerator LoadCreditAsync()
        {
            ComponentReferenceHolder.CreditFromTitle = _currentLoadedScene == SceneType.Title;
            yield return UnloadEntireCurrentSceneAsync();
            SceneManager.sceneLoaded += LoadSceneEnd;
            _leftLoadSceneCount = 1;
            yield return LoadPrograssSingleSceneAsync("Title", "Scenes/CreditScene/Credit");
            _currentLoadedScene = SceneType.Credit;
        }

        private IEnumerator LoadGameAsync()
        {
            OnStageLoad?.Invoke();
            yield return UnloadEntireCurrentSceneAsync();

            SceneManager.sceneLoaded += LoadSceneEnd;
            var loadScenes = new[]
            {
                new SceneInfo("EventSystem", "Scenes/GameScene/EventSystem"),
                new SceneInfo("Stage",  GameManager.Storage!.GameData!.CurrentSceneName),
                new SceneInfo("Camera", "Scenes/GameScene/Camera"),
                new SceneInfo("GameUI", "Scenes/GameScene/GameUI")
            };
            _leftLoadSceneCount = loadScenes.Length;

            IsStageLoading = true;
            StageInitializeStart();
            yield return LoadPrograssScenesAsync(loadScenes);
            LoadUi!.Text = "Game Save State";
            yield return null;
            StageInitializeEnd();
            OnStageLoaded?.Invoke();
            _currentLoadedScene = SceneType.Game;
            IsStageLoading = false;
        }

        private IEnumerator ChangeStageAsync(string stageName, ShadowTransitionDirection shadowTransitionDirection)
        {
            OnStageLoad?.Invoke();
            OnStageChange?.Invoke();
            IsStageChanging = true;
            GameManager.Storage!.SceneDataSync();
            var isShowTransitionCompleted = false;
            _sceneLoader!.LoadUi!.Show(ShadowTransitionDirection2LoadingType(shadowTransitionDirection),
                () => isShowTransitionCompleted = true);
            while (!isShowTransitionCompleted)
                yield return null;
            yield return UnloadSceneAsync("Unloading Stage", GameManager.Storage!.GameData!.CurrentSceneName);
            SceneManager.sceneLoaded += LoadSceneEnd;
            _leftLoadSceneCount = 1;
            GameManager.Storage.GameData.CurrentSceneName = stageName;
            StageInitializeStart();
            var asyncOperation = SceneManager.LoadSceneAsync(stageName, LoadSceneMode.Additive);
            while (!asyncOperation.isDone)
                yield return null;
            StageInitializeEnd();
            OnStageLoaded?.Invoke();
            OnStageChanged?.Invoke();
            IsStageChanging = false;
        }

        private static void StageInitializeStart() => ComponentReferenceHolder.PositionSavers = new Dictionary<string, Transform>();

        private static void StageInitializeEnd()
        {
            GameManager.Storage!.GameData!.GlobalState.InitializeState();
            foreach (var item in GameManager.Storage.GameData.GetScenePositions(GameManager.Storage.GameData.CurrentSceneName))
                if (ComponentReferenceHolder.PositionSavers!.TryGetValue(item.PositionId, out var value))
                    value.position = (Vector2)item.Position;
            OnStageInitalize?.Invoke();
            ComponentReferenceHolder.Reset();
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private IEnumerator LoadPrograssScenesAsync(SceneInfo[] sceneInfos)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < sceneInfos.Length; index++)
            {
                var item = sceneInfos[index];
                yield return LoadPrograssSingleSceneAsync(item.UiText, item.SceneName, sceneInfos.Length, index + 1);
            }
        }

        private IEnumerator LoadPrograssSingleSceneAsync(string loadText, string sceneName)
        {
            yield return LoadPrograssSingleSceneAsync(loadText, sceneName, 1, 1);
        }

        private IEnumerator LoadPrograssSingleSceneAsync(string loadText, string sceneName, int loadSceneCount, int currentScneneCount)
        {
            LoadUi!.Text = loadText;
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            var elapsedTime = 0.0f;
            while (!asyncOperation.isDone)
            {
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
                var prevLoadedScenesOffset = (1f / loadSceneCount) * (currentScneneCount - 1);
                if (asyncOperation.progress < 0.9f)
                {
                    LoadUi.Prograss = Mathf.Lerp(
                        (LoadUi.Prograss - prevLoadedScenesOffset) * loadSceneCount,
                        asyncOperation.progress,
                        elapsedTime) / loadSceneCount + prevLoadedScenesOffset;
                    if ((LoadUi.Prograss - prevLoadedScenesOffset) * loadSceneCount >= asyncOperation.progress)
                        elapsedTime = 0f;
                }
                else
                {
                    LoadUi.Prograss = Mathf.Lerp(
                        (LoadUi.Prograss - prevLoadedScenesOffset) * loadSceneCount,
                        1f,
                        elapsedTime) / loadSceneCount + prevLoadedScenesOffset;
                    if (Math.Abs((LoadUi.Prograss - prevLoadedScenesOffset) * loadSceneCount - 1.0f) > 0.00001f) continue;
                    asyncOperation.allowSceneActivation = true;
                    break;
                }
            }
        }

        private IEnumerator UnloadEntireCurrentSceneAsync()
        {
            switch (_currentLoadedScene)
            {
                case SceneType.None:
                    break;
                case SceneType.Title:
                    yield return UnloadSceneAsync("Unloading Title", "Scenes/TitleScene/Title");
                    break;
                case SceneType.Game:
                    yield return UnloadSceneAsync("Unloading GameUI", "Scenes/GameScene/GameUI");
                    yield return UnloadSceneAsync("Unloading Camera", "Scenes/GameScene/Camera");
                    yield return UnloadSceneAsync("Unloading Stage", GameManager.Storage!.GameData!.CurrentSceneName);
                    yield return UnloadSceneAsync("Unloading Title", "Scenes/GameScene/EventSystem");
                    break;
                case SceneType.Credit:
                    yield return UnloadSceneAsync("Unloading Credit", "Scenes/CreditScene/Credit");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator UnloadSceneAsync(string unloadText, string sceneName)
        {
            LoadUi!.Text = unloadText;
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            while (!asyncOperation.isDone)
                yield return null;
        }

        private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
        {
            _leftLoadSceneCount -= 1;
            if (_leftLoadSceneCount != 0) return;
            LoadUi!.Hide();
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }

        private readonly struct SceneInfo
        {
            public readonly string UiText;
            public readonly string SceneName;
            public SceneInfo(string uiText, string sceneName)
                => (UiText, SceneName) = (uiText, sceneName);
        }

        private LoadUi.LoadingType ShadowTransitionDirection2LoadingType(
            ShadowTransitionDirection shadowTransitionDirection)
            => shadowTransitionDirection switch
            {
                ShadowTransitionDirection.Up => LoadUi.LoadingType.UpShadow,
                ShadowTransitionDirection.Down => LoadUi.LoadingType.DownShadow,
                ShadowTransitionDirection.Left => LoadUi.LoadingType.LeftShadow,
                ShadowTransitionDirection.Right => LoadUi.LoadingType.RightShadow,
                _ => throw new ArgumentOutOfRangeException(),
            };

        private enum SceneType
        {
            None,
            Title,
            Game,
            Credit
        }

        public enum ShadowTransitionDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
