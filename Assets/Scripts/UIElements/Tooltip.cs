using UnityEngine;

namespace UIElements
{
    public class Tooltip : MonoBehaviour
    {
        public bool IsActive => gameObject.activeSelf;
        
        private void Start()
        {
            HideTooltip();
        }
        
        public void ShowTooltip()
        {
            gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }
    }
}