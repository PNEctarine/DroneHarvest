using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using Source.Code.ECS.Components;
using Source.Code.MonoBehaviours;
using Source.Code.MonoBehaviours.Starships.Behaviours;
using Source.Code.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Source.Code.ECS.Systems
{
    public class StarshipsSpawnSystem: IEcsInitSystem
    {
        [Inject] private StarshipsConfig _starshipsConfig;

        private EcsFilter _locationComponentFilter;
        private EcsPool<LocationComponent> _locationComponentPool;
        
        private LocationComponent _locationComponent;
        
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            
            _locationComponentFilter = world.Filter<LocationComponent>().End();
            _locationComponentPool = world.GetPool<LocationComponent>();
            
            foreach (int i in _locationComponentFilter)
            {
                _locationComponent = _locationComponentPool.Get(i);
            }

            int shipsLength = _locationComponent.GrayTeamLaunchingPads.Length;
            
            for (int i = 0; i < shipsLength; i++)
            {
                SpawnStarship(world, _starshipsConfig.GreyTeamStarshipsPrefabs, _locationComponent.GrayTeamLaunchingPads[i], i, FractionType.Gray);
                SpawnStarship(world, _starshipsConfig.RedTeamStarshipsPrefabs, _locationComponent.RedTeamLaunchingPads[i], i, FractionType.Red);
            }
        }

        private void SpawnStarship(EcsWorld world, GameObject[] prefabs, LaunchingPad launchingPad, int index, FractionType team)
        {
            int randomIndex = Random.Range(0, prefabs.Length);
            GameObject starship = Object.Instantiate(prefabs[randomIndex]);
            starship.TryGetComponent(out StarshipBehaviour starshipBehaviour);

            if (starshipBehaviour != null)
            {
                EcsConverter.ConvertContainer(starship.GetComponent<ComponentsContainer>(), world);
                
                starshipBehaviour.NavMeshAgent.enabled = false;

                starshipBehaviour.transform.position = launchingPad.SpawnPoint.position;
                starshipBehaviour.transform.rotation = launchingPad.SpawnPoint.rotation;

                starshipBehaviour.StarshipViewTransform.position = new Vector3(starshipBehaviour.StarshipViewTransform.position.x,
                    launchingPad.SpawnPoint.position.y,
                    starshipBehaviour.StarshipViewTransform.position.z);

                starshipBehaviour.Init(launchingPad.SpawnPoint.position, index);

                EcsPool<StarshipComponent> starshipComponentPool = world.GetPool<StarshipComponent>();
                EcsFilter starshipComponentFilter = world.Filter<StarshipComponent>().End();
                
                foreach (int i in starshipComponentFilter)
                {
                    ref StarshipComponent comp = ref starshipComponentPool.Get(i);
                    
                    if (comp.StarshipBehaviour == starshipBehaviour)
                    {
                        comp.SetFaction(team);
                        comp.CarryingResourceEntity = -1;
                        comp.State = Enums.StarshipState.IdleOnPad;
                        comp.StateTimer = 0f;
                        break;
                    }
                }
            }

            else
            {
                Debug.LogError("StarshipBehaviour not found");
            }
        }
    }
}
