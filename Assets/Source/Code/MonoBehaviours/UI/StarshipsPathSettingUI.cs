using Source.Code.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Source.Code.MonoBehaviours.UI
{
    public class StarshipsPathSettingUI : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;

        [Inject] private SessionData _sessionData;
    
        private void Awake()
        {
            _toggle.isOn = false;
            
            _toggle.onValueChanged.AddListener(value =>
            {
                _sessionData.IsHowStarshipsPath = value;
            });
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveAllListeners();
        }
    }
}
