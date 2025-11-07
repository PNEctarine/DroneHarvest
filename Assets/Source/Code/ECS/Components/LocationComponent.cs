using System;
using Source.Code.MonoBehaviours;
using UnityEngine;

namespace Source.Code.ECS.Components
{
    [Serializable]
    public struct LocationComponent
    {
        [field: SerializeField] public LaunchingPad[] GrayTeamLaunchingPads { get; private set; }
        [field: SerializeField] public LaunchingPad[] RedTeamLaunchingPads { get; private set; }
        [field: SerializeField] public Collider ResourcesSpawnZone { get; private set; }
    }
}
