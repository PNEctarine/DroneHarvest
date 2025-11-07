using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.MonoBehaviours.UI
{
    public class PauseWindow : BaseWindow
    {
        [Space(10)]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quitButton;
        
        public override void Init()
        {
            if (IsInit == false)
            {
                _resumeButton.onClick.AddListener(Hide);
                _quitButton.onClick.AddListener(Application.Quit);

                IsInit = true;
                
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
        }
    }
}
