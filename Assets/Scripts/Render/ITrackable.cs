using UnityEngine;

namespace Assets.Scripts.Render
{
    public interface ITrackable : IDirectionable
    {
        public Vector3 Position { get; }
        public Vector2 MoveVector { get; }
    }
}
