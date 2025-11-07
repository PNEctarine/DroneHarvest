using UnityEngine;

namespace Source.Code.MonoBehaviours.UI
{
    public class GameUI : MonoBehaviour
    {
        [field: SerializeField] public GameScreenUI GameScreenUI { get; private set; }
    }
}
