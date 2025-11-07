using UnityEngine;

namespace Source.Code.ScriptableObjects
{
    [CreateAssetMenu(fileName = "StarshipsConfig", menuName = "Configs/StarshipsConfig")]
    public class StarshipsConfig : ScriptableObject
    {
        [field: SerializeField] public GameObject[] GreyTeamStarshipsPrefabs;
        [field: SerializeField] public GameObject[] RedTeamStarshipsPrefabs;
    }
}
