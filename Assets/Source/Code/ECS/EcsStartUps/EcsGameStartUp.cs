using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using Source.Code.Data;
using Source.Code.ECS.Systems;
using UnityEngine;
using Zenject;

namespace Source.Code.ECS.EcsStartUps
{
    public class EcsGameStartUp : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private GameEvents _gameEvents;
        
        private EcsWorld _world;

        private EcsSystems _initSystems;
        private EcsSystems _updateSystems;
        private EcsSystems _fixedUpdateSystems;
        
        private void Start()
        {
            _world = new EcsWorld();
            _initSystems = new EcsSystems(_world);
            _updateSystems = new EcsSystems(_world);
            _fixedUpdateSystems = new EcsSystems(_world);
            
            AddInitSystems();
            AddUpdateSystems();
        }
        
        private void AddInitSystems()
        {
            _initSystems
                .Add(_diContainer.Instantiate<StarshipsSpawnSystem>())
                
                .ConvertScene()
                .Init();
            
        }
        
        private void AddUpdateSystems()
        {
            _updateSystems
                .Add(_diContainer.Instantiate<StarshipsMoveSystem>())
                .Add(_diContainer.Instantiate<ResourcesSpawnSystem>())
                
                .Init();
        }
        
        private void Update()
        {
            _updateSystems?.Run();
        }
        
        private void OnDestroy()
        {
            _initSystems.Destroy();
            _fixedUpdateSystems.Destroy();
            _updateSystems.Destroy();
            _world.Destroy();
            _world = null;

            _gameEvents.ClearAllEvents();
        }
    }
}
