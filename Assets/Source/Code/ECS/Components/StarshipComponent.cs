using System;
using Source.Code.Enums;
using Source.Code.MonoBehaviours.Starships.Behaviours;
using UnityEngine;

namespace Source.Code.ECS.Components
{
    [Serializable]
    public struct StarshipComponent
    {
        [field: SerializeField] public StarshipBehaviour StarshipBehaviour { get; private set; }
        [field: SerializeField] public Transform TrunkTransform { get; private set; }
        [field: SerializeField] public FractionType Faction { get; private set; }
        
        [HideInInspector] public StarshipState State;
        [HideInInspector] public float StateTimer;
        [HideInInspector] public int CarryingResourceEntity;

        public void SetFaction(FractionType value)
        {
            Faction = value;
        }
    }
}
