using UnityEngine;

namespace Source.Code.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ResourcesConfig", menuName = "Configs/ResourcesConfig")]
    public class ResourcesConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public GameObject[] Resources { get; private set; }
    }
}
