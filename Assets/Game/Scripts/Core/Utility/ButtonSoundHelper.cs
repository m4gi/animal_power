using Game.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundHelper : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
        
        _button.onClick.AddListener(PlayClickButton);
    }

    private void PlayClickButton()
    {
        SoundSystem.Instance?.PlaySFX("button_click");
    }
}
