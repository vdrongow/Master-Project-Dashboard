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
            if(gameObject.activeSelf)
                return;
            gameObject.SetActive(true);
            var gameManager = GameManager.Singleton;
            if(!gameManager.Stats.TryAdd("infoIconClicked", "1"))
            {
                gameManager.Stats["infoIconClicked"] = (int.Parse(gameManager.Stats["infoIconClicked"]) + 1).ToString();
            }
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }
    }
}