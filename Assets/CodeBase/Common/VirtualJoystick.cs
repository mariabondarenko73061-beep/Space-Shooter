using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image m_JoyBack;
        [SerializeField] private Image m_JoyStick;

        public Vector3 Value { get; private set; }

        private void Awake()
        {
            if (m_JoyBack == null)
                m_JoyBack = GetComponent<Image>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_JoyBack == null) return;

            RectTransform backRect = m_JoyBack.rectTransform;
            Vector2 size = backRect.sizeDelta;

            if (Mathf.Approximately(size.x, 0f) || Mathf.Approximately(size.y, 0f))
                return;

            Vector2 position = Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(backRect,
            eventData.position,
            eventData.pressEventCamera,
            out position);

            position.x = (position.x / size.x);
            position.y = (position.y / size.y);

            position.x = position.x * 2 - 1;
            position.y = position.y * 2 - 1;

            Value = new Vector3(position.x, position.y, 0);

            if (Value.magnitude > 1)
                Value = Value.normalized;

            if (m_JoyStick == null) return;

            float offsetX = size.x / 2 - m_JoyStick.rectTransform.sizeDelta.x / 2;
            float offsetY = size.y / 2 - m_JoyStick.rectTransform.sizeDelta.y / 2;

            m_JoyStick.rectTransform.anchoredPosition = new Vector2(Value.x * offsetX, Value.y * offsetY);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector3.zero;

            if (m_JoyStick != null)
                m_JoyStick.rectTransform.anchoredPosition = Vector3.zero;
        }
    }
}