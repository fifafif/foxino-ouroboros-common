using UnityEngine;
using UnityEngine.EventSystems;

namespace Ouroboros.Common.Inputs
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public Vector2 InputVector { get; private set; }

        [SerializeField] private RectTransform joystickKnob;
        [SerializeField] private RectTransform joystickBackground;

        private void Start()
        {
            if (joystickBackground == null)
            {
                joystickBackground = GetComponent<RectTransform>();
            }

            if (joystickKnob == null)
            {
                joystickKnob = transform.GetChild(0).GetComponent<RectTransform>();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBackground, eventData.position, eventData.pressEventCamera, out var position);

            var joystickWidth = joystickBackground.rect.width;
            var joystickHeight = joystickBackground.rect.height;

            if (joystickWidth <= 0f
                || joystickHeight <= 0f)
            {
                Debug.LogError($"Joystick has no width or height!", this);
                return;
            }

            position.x /= joystickWidth * 0.5f;
            position.y /= joystickHeight * 0.5f;

            InputVector = position;
            InputVector = InputVector.magnitude > 1.0f ? InputVector.normalized : InputVector;

            joystickKnob.anchoredPosition = new Vector2(
                InputVector.x * (joystickWidth / 2), 
                InputVector.y * (joystickHeight / 2));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputVector = Vector2.zero;
            joystickKnob.anchoredPosition = Vector2.zero;
        }
    }
}