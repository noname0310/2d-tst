#nullable enable
using System;
using UnityEngine;
using Assets.Scripts.Render;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class MovementAnimationController : MonoBehaviour, ITrackable
    {
        public float PlayAnimationMinForce = 0.5f;
        private Rigidbody2D? _rigidbody2D;
        private Animator? _animator;
        private PlayerMovementController? _playerController;
        private AiPathController? _aiPathController;
        private MoveAnimationState _animationState;
        private MoveAnimationState _playingAnimationState;
        private static int[]? _animationStateHashes;

        public Vector3 Position => transform.position;

        public Vector2 LookVector => _playingAnimationState switch
        {
            MoveAnimationState.IdleUp => Vector2.up,
            MoveAnimationState.IdleDown => Vector2.down,
            MoveAnimationState.IdleRight => Vector2.right,
            MoveAnimationState.IdleLeft => Vector2.left,
            MoveAnimationState.MoveUp => Vector2.up,
            MoveAnimationState.MoveDown => Vector2.down,
            MoveAnimationState.MoveRight => Vector2.right,
            MoveAnimationState.MoveLeft => Vector2.left,
            _ => throw new ArgumentOutOfRangeException()
        };

        public Vector2 MoveVector => _aiPathController != null && _aiPathController.IsPathing
            ? (Vector2)_aiPathController.DesiredVelocity
            : _rigidbody2D!.velocity;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _animationStateHashes ??= new[]
            {
                Animator.StringToHash("IdleUp"),
                Animator.StringToHash("IdleDown"),
                Animator.StringToHash("IdleRight"),
                Animator.StringToHash("IdleLeft"),
                Animator.StringToHash("MoveUp"),
                Animator.StringToHash("MoveDown"),
                Animator.StringToHash("MoveRight"),
                Animator.StringToHash("MoveLeft")
            };
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _playerController = GetComponent<PlayerMovementController>();
            _aiPathController = GetComponent<AiPathController>();
            _animationState = MoveAnimationState.IdleDown;
            _playingAnimationState = MoveAnimationState.IdleDown;
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            var vector = _aiPathController != null && _aiPathController.IsPathing
                ? (Vector2)_aiPathController.DesiredVelocity
                : _rigidbody2D!.velocity;

            var doIdle = false;

            if (vector.x < vector.y)
            {
                if (-vector.x < vector.y)
                {
                    if (PlayAnimationMinForce < vector.y)
                        _animationState = MoveAnimationState.MoveUp;
                    else
                        doIdle = true;
                }
                else if (vector.x < -PlayAnimationMinForce)
                    _animationState = MoveAnimationState.MoveLeft;
                else
                    doIdle = true;
            }
            else
            {
                if (-vector.y < vector.x)
                {
                    if (PlayAnimationMinForce < vector.x)
                        _animationState = MoveAnimationState.MoveRight;
                    else
                        doIdle = true;
                }
                else if (vector.y < -PlayAnimationMinForce)
                    _animationState = MoveAnimationState.MoveDown;
                else
                    doIdle = true;
            }

            if (doIdle)
            {
                _animationState = _playingAnimationState switch
                {
                    MoveAnimationState.MoveUp => MoveAnimationState.IdleUp,
                    MoveAnimationState.MoveDown => MoveAnimationState.IdleDown,
                    MoveAnimationState.MoveRight => MoveAnimationState.IdleRight,
                    MoveAnimationState.MoveLeft => MoveAnimationState.IdleLeft,
                    _ => _animationState
                };

                var moveVector = Vector2.zero;
                if (_playerController != null)
                    moveVector = _playerController.MoveVector;
                else if (_aiPathController != null && _aiPathController.IsPathing)
                {
                    var desiredVelocity = _aiPathController.DesiredVelocity;
                    moveVector = Mathf.Abs(desiredVelocity.x) < Mathf.Abs(desiredVelocity.y)
                        ? new Vector2(0, desiredVelocity.y).normalized
                        : new Vector2(desiredVelocity.x, 0).normalized;
                }

                if (moveVector != Vector2.zero)
                {
                    if (moveVector == Vector2.up && _playingAnimationState != MoveAnimationState.IdleUp)
                        _animationState = MoveAnimationState.IdleUp;
                    else if (moveVector == Vector2.down && _playingAnimationState != MoveAnimationState.IdleDown)
                        _animationState = MoveAnimationState.IdleDown;
                    else if (moveVector == Vector2.left && _playingAnimationState != MoveAnimationState.IdleLeft)
                        _animationState = MoveAnimationState.IdleLeft;
                    else if (moveVector == Vector2.right && _playingAnimationState != MoveAnimationState.IdleRight)
                        _animationState = MoveAnimationState.IdleRight;
                }
            }

            if (_animationState == _playingAnimationState) return;
            _animator!.Play(_animationStateHashes![(int)_animationState]);
            _playingAnimationState = _animationState;
        }

        public enum MoveAnimationState
        {
            IdleUp,
            IdleDown,
            IdleRight,
            IdleLeft,
            MoveUp,
            MoveDown,
            MoveRight,
            MoveLeft
        }
    }
}
