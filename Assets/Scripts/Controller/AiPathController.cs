#nullable enable
using UnityEngine;
using Pathfinding;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIPath), typeof(AIDestinationSetter))]
    public class AiPathController : MonoBehaviour
    {
        public bool IsPathing => _isForcePathing || _aiDestinationSetter!.enabled;

        public Vector3 DesiredVelocity
        {
            get
            {
                if (_aiDestinationSetter!.enabled)
                    return _aiPath!.desiredVelocity;
                return _forcePathingVelocity;
            }
        }

        private AIPath? _aiPath;
        private AIDestinationSetter? _aiDestinationSetter;
        private bool _isForcePathing;
        private Vector2 _forcePathingVelocity;
        private const float ForceMoveSpeed = 0.8f;
        private const int LinearDragOffset = 100;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _aiPath = GetComponent<AIPath>();
            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
            _aiPath.enabled = false;
            _aiDestinationSetter.enabled = false;
            _isForcePathing = false;
            _forcePathingVelocity = Vector2.zero;
        }

        public void SetPath(Transform target)
        {
            _aiDestinationSetter!.target = target;
            _aiPath!.enabled = true;
            _aiDestinationSetter!.enabled = true;
        }

        public void StopPath()
        {
            _aiPath!.enabled = false;
            _aiDestinationSetter!.enabled = false;
        }

        public void ForceMoveStep(Vector3 target, float deltaTime)
        {
            _isForcePathing = true;
            _forcePathingVelocity = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, target, ForceMoveSpeed * deltaTime);
            _forcePathingVelocity = (Vector2)transform.position - _forcePathingVelocity;
            _forcePathingVelocity *= LinearDragOffset;
        }

        public void ForceMoveEnd() => _isForcePathing = false;
    }
}
