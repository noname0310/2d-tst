#nullable enable
using UnityEngine;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementController : MonoBehaviour
    {
        public float MoveSpeed = 700f;
        public float SqMinVector = 0.1f;

        public Vector2 MoveVector { get; private set; }
        private Rigidbody2D? _rigidbody2D;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            MoveVector = Vector2.zero;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            GameManager.Ui!.OnMove += UiEventManager_OnMove;
        }

        // ReSharper disable once UnusedMember.Local
        private void FixedUpdate() => _rigidbody2D!.velocity = MoveVector * (MoveSpeed * Time.fixedDeltaTime);

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (GameManager.Ui != null)
                GameManager.Ui.OnMove -= UiEventManager_OnMove;
        }

        private void UiEventManager_OnMove(Vector2 vector2)
        {
            var sqrMagnitude = vector2.sqrMagnitude;
            if (-SqMinVector <= sqrMagnitude && sqrMagnitude <= SqMinVector)
                MoveVector = Vector2.zero;
            else
                MoveVector = Mathf.Abs(vector2.x) < Mathf.Abs(vector2.y)
                    ? new Vector2(0, vector2.y).normalized
                    : new Vector2(vector2.x, 0).normalized;
        }
    }
}
