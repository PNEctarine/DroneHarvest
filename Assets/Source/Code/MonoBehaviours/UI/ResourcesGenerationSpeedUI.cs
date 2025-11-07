using System.Globalization;
using Source.Code.Data;
using TMPro;
using UnityEngine;
using Zenject;

namespace Source.Code.MonoBehaviours.UI
{
    public class ResourcesGenerationSpeedUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TMP_Text _valueText;

        [Inject] private SessionData _sessionData;

        private void Awake()
        {
            _inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            _sessionData.ResourceSpawnInterval = 5f;
            _valueText.text = _sessionData.ResourceSpawnInterval.ToString(CultureInfo.InvariantCulture);
            
            _inputField.onValueChanged.AddListener(value =>
            {
                if(float.TryParse(value, out float result))
                {
                    _sessionData.ResourceSpawnInterval = result;
                    _valueText.text = result.ToString(CultureInfo.InvariantCulture);
                }
                
                else
                {
                    Debug.Log($"Attempted conversion of {value} failed");
                }
            });
        }

        private void OnDestroy()
        {
            _inputField.onValueChanged.RemoveAllListeners();
        }
    }
}
