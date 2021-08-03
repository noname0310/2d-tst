#nullable enable
using UnityEngine;
using Assets.Scripts.Event;
using Assets.Scripts.Render;

namespace Assets.Scripts.Controller
{
    [DisallowMultipleComponent]
    public class PlayerRaycastController : MonoBehaviour
    {
        public float Distance = 0.6f;
        public float CenterRadius = 0.5f;
        private IDirectionable? _directionable;

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _directionable = GetComponent<IDirectionable>();
            GameManager.Ui!.OnInteractionButtonClicked += Ui_OnInteractionButtonClicked;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDestroy()
        {
            if (GameManager.Ui != null) 
                GameManager.Ui.OnInteractionButtonClicked -= Ui_OnInteractionButtonClicked;
        }

        private void Ui_OnInteractionButtonClicked()
        {
            var raycastHit2D = Physics2D.Raycast(
                (Vector2)transform.position + _directionable!.LookVector.normalized * CenterRadius, 
                _directionable.LookVector, 
                Distance, 
                int.MaxValue);
            if (raycastHit2D.transform == null) return;
            var eventTriggers = raycastHit2D.transform.gameObject.GetComponents<EventTrigger>();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < eventTriggers.Length; i++)
            {
                var eventTrigger = eventTriggers[i];
                if (eventTrigger.Event.TriggerType != Event.Event.EventTriggerType.Raycast) return;
                eventTrigger.InvokeTrigger(gameObject);
            }
        }
    }
}
