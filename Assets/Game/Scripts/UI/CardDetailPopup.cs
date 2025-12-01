using System.Text;
using Game.Scripts.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class CardDetailPopup : MonoBehaviour
    {
        [SerializeField] private Image infoImage;
        [SerializeField] private TextMeshProUGUI powerText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI skillText;

        public void InitDetail(AnimalConfig config)
        {
            infoImage.sprite = config.animalSprite;
            powerText.text = $"<color=#B3B5C7><size=65%>Power</size></color>\n{config.strength}";
            costText.text = $"<color=#B3B5C7><size=65%>Cost</size></color>\n{config.animalLevel}";
            string description = config.skillDescription;
            StringBuilder sb = new StringBuilder();
            sb.Append("Skill: ");
            sb.Append(string.IsNullOrEmpty(description) ? "No Skill" : $"<color=#a8a632>{description}</color>");
            skillText.text = sb.ToString();
        }
    }
}