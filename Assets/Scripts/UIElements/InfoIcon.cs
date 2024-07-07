using UnityEngine;
using UnityEngine.EventSystems;

namespace UIElements
{
    public class InfoIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Tooltip tooltip;

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.HideTooltip();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (tooltip.IsActive)
            {
                tooltip.HideTooltip();
            }
            else
            {
                tooltip.ShowTooltip();
            }
        }
    }
}