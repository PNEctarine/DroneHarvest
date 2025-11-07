using System.Globalization;
using Source.Code.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Source.Code.MonoBehaviours.UI
{
    public class StarshipsSpeedSettingUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _valueText;

        [Inject] private SessionData _sessionData;
        [Inject] private GameEvents _gameEvents;

        private void Awake()
        {
            _slider.minValue = 1;
            _slider.wholeNumbers = true;
            _sessionData.ShipSpeed = 1;
            
            _slider.onValueChanged.AddListener(value =>
            {
                ChangeValue();
                _gameEvents.OnDronesSpeedChanged?.Invoke();
            });
            
            ChangeValue();
        }
        
        private void ChangeValue()
        {
            _sessionData.ShipSpeed = (int)_slider.value;
            _valueText.text = _sessionData.ShipSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveAllListeners();
        }
    }
}
