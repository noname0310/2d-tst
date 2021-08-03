using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    public class StickUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public bool BlockingInput { get; set; } = false;
        public float MovementRange = 150;
        private Vector3 _startPos;
        private Vector2 _pointerDownPos;

        // ReSharper disable once UnusedMember.Local
        private void Start() => _startPos = ((RectTransform) transform).anchoredPosition;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponentInParent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out _pointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponentInParent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out var position);
            var delta = position - _pointerDownPos;

            delta = Vector2.ClampMagnitude(delta, MovementRange);
            ((RectTransform)transform).anchoredPosition =_startPos + (Vector3)delta;

            if (BlockingInput) return;
            var controllerVector = new Vector2(delta.x / MovementRange, delta.y / MovementRange);
            GameManager.Ui!.ControllerLeftStickMove(controllerVector);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition = _startPos;
            if (BlockingInput) return; 
            GameManager.Ui!.ControllerLeftStickMove(Vector2.zero);
        }
    }
}
