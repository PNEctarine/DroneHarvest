using System.Globalization;
using Source.Code.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Source.Code.MonoBehaviours.UI
{
    public class StarshipsCountSettingUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _valueText;

        [Inject] private SessionData _sessionData;

        private void Awake()
        {
            _slider.minValue = 1;
            _slider.wholeNumbers = true;
            _sessionData.ActiveShipsCount = 1;
            _valueText.text = _sessionData.ActiveShipsCount.ToString(CultureInfo.InvariantCulture);
            
            _slider.onValueChanged.AddListener(value =>
            {
                _sessionData.ActiveShipsCount = (int)value;
                _valueText.text = value.ToString(CultureInfo.InvariantCulture);
            });
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveAllListeners();
        }
    }
}
