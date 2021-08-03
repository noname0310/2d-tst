#nullable enable
using System;
using UnityEngine;
using Assets.Scripts.Storage.Model;

namespace Assets.Scripts.Loader
{
    [DisallowMultipleComponent]
    public class PositionSaver : MonoBehaviour
    {
        public string? PositionId;

        // ReSharper disable once UnusedMember.Global
        public void Awake()
        {
            ComponentReferenceHolder.PositionSavers!.Add(PositionId ?? throw new ArgumentNullException(), transform);
            GameManager.Storage!.OnCollectGameSaveInfo += Storage_OnCollectGameSaveInfo;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (GameManager.Storage != null) GameManager.Storage.OnCollectGameSaveInfo -= Storage_OnCollectGameSaveInfo;
        }

        private void Storage_OnCollectGameSaveInfo() =>
            GameManager.Storage!.GameData!.AddPosition(
                ScenePathConverter.GetLoadableScenePath(gameObject.scene.path), 
                new GameData.SavePosition(PositionId ?? throw new ArgumentNullException(), (Vector2)transform.position));
    }
}
