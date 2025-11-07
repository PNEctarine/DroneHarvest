using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.MonoBehaviours.UI
{
    public class GameScreenUI : MonoBehaviour
    {
        
        [field: SerializeField] public ScoreUI GrayTeamScore { get; private set; }
        [field: SerializeField] public ScoreUI RedTeamScore { get; private set; }
        
        [field: Space(10)]
        [field: SerializeField] public Button PauseButton { get; private set; }
        
        [SerializeField] private BaseWindow _pauseWindow;
        
        private readonly Dictionary<Button, BaseWindow> _buttonsMap = new();

        private void Awake()
        {
            _buttonsMap.Add(PauseButton, _pauseWindow);

            foreach (KeyValuePair<Button, BaseWindow> a in _buttonsMap)
            {
                a.Value.Init();
                a.Key.onClick.AddListener(() =>
                {
                    a.Value.Show();
                });
                _buttonsMap[a.Key].Init();
            }
        }
    }
}
