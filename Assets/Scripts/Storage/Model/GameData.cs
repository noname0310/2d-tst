using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Event;

namespace Assets.Scripts.Storage.Model
{
    [Serializable]
    public class GameData
    {
        public StateManager GlobalState { get; }
        public IntegrityDictionary<string, StateManager> LocalStates { get; }
        public string CurrentSceneName { get; set; }
        private readonly Dictionary<string, List<SavePosition>> _savePositions;

        private GameData(StateManager globalStateManager, IntegrityDictionary<string, StateManager> localStateManagers, string currentSceneName) =>
            (GlobalState, LocalStates, CurrentSceneName, _savePositions) 
            = (globalStateManager, localStateManagers, currentSceneName, new Dictionary<string, List<SavePosition>>());

        public static GameData BuildDefault(string startStageName) => new GameData(new StateManager(), new IntegrityDictionary<string, StateManager>(), startStageName);

        public void AddPosition(string sceneName, SavePosition savePosition)
        {
            if (!_savePositions.TryGetValue(sceneName, out var sceneSavePositions))
            {
                sceneSavePositions = new List<SavePosition>();
                _savePositions.Add(sceneName, sceneSavePositions);
            }
            sceneSavePositions.Add(savePosition);
        }

        public List<SavePosition> GetScenePositions(string sceneName)
        {
            if (_savePositions.TryGetValue(sceneName, out var sceneSavePositions)) return sceneSavePositions;
            sceneSavePositions = new List<SavePosition>();
            _savePositions.Add(sceneName, sceneSavePositions);
            return sceneSavePositions;
        }

        public void ClearScenePositions(string sceneName) => GetScenePositions(sceneName).Clear();

        [Serializable]
        public readonly struct SavePosition
        {
            public readonly string PositionId;
            public readonly SerializableVector2 Position;

            public SavePosition(string positionId, SerializableVector2 position) => (PositionId, Position) = (positionId, position);

            [Serializable]
            public struct SerializableVector2
            {
                public readonly float X;
                public readonly float Y;

                public SerializableVector2(float x, float y) => (X, Y) = (x, y);

                public static explicit operator Vector2(SerializableVector2 serializableVector2) =>
                    new Vector2(serializableVector2.X, serializableVector2.Y);

                public static implicit operator SerializableVector2(Vector2 vector2) =>
                    new SerializableVector2(vector2.x, vector2.y);
            }
        }
    }
}
