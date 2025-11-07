using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.MonoBehaviours.UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] protected Image _background;
        [SerializeField] protected Image _windowBackground;
        
        protected bool IsInit;

        public abstract void Init();
        
        public void Show()
        {
            Sequence openSequence = DOTween.Sequence();

            openSequence
                .Append(_windowBackground.transform.DOScale(1.2f, 0.2f))
                .Append(_windowBackground.transform.DOScale(1f, 0.1f));
            
            gameObject.SetActive(true);
            openSequence.Play();
            
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 0);
            _background.DOFade(0.97f, 0.3f);
        }

        protected void Hide()
        {
            Sequence closeSequence = DOTween.Sequence();

            closeSequence.Append(_windowBackground.transform.DOScale(0, 0.2f));
            closeSequence.Play();
            
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 0.97f);

            _background.DOFade(0, 0.3f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
