using UnityEngine;
using UnityEngine.EventSystems;

namespace Ouroboros.Common.Utils.UI.Tests
{
    public class UIClickTest : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"OnPointerClick", this);
        }
    }
}