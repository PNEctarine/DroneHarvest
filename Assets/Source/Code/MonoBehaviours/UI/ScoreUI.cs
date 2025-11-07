using TMPro;
using UnityEngine;

namespace Source.Code.MonoBehaviours.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _score;

        private void Start()
        {
            _score.text = "0";
        }

        public void SetScore(int score)
        {
            _score.text = score.ToString();
        }
    }
}
