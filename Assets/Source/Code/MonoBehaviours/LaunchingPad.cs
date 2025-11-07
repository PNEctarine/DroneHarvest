using UnityEngine;

namespace Source.Code.MonoBehaviours
{
    public class LaunchingPad : MonoBehaviour
    {
        [field: SerializeField] public Transform SpawnPoint { get; private set; }
    }
}
