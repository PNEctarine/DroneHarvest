using Leopotam.EcsLite;
using Source.Code.Data;
using Source.Code.ECS.Components;
using Source.Code.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Source.Code.ECS.Systems
{
    public class ResourcesSpawnSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        [Inject] private SessionData _sessionData;
        [Inject] private GameEvents _gameEvents;
        [Inject] private ResourcesConfig _resourcesConfig;
        [Inject] private DiContainer _diContainer;

        private EcsFilter _locationComponentFilter;
        private EcsPool<LocationComponent> _locationComponentPool;

        private EcsWorld _world;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _locationComponentFilter = _world.Filter<LocationComponent>().End();
            _locationComponentPool = _world.GetPool<LocationComponent>();
            
            _gameEvents.OnDronesSpeedChanged += ChangeSpawnInterval;
        }

        public void Run(IEcsSystems systems)
        {
            _sessionData.ResourceSpawnTimer -= Time.deltaTime;
            
            if (_sessionData.ResourceSpawnTimer > 0)
                return;
            
            foreach (int i in _locationComponentFilter)
            {
                ref LocationComponent locationComponent = ref _locationComponentPool.Get(i);

                if (!TryGetSpawnPoint(locationComponent.ResourcesSpawnZone, out Vector3 spawnPoint))
                    continue;

                GameObject prefab = _resourcesConfig.Resources[Random.Range(0, _resourcesConfig.Resources.Length)];
                GameObject instancePrefab = _diContainer.InstantiatePrefab(prefab, spawnPoint, Quaternion.identity, null);
                
                int e = _world.NewEntity();
                EcsPool<ResourceComponent> pool = _world.GetPool<ResourceComponent>();
                
                ref ResourceComponent resourceComponent = ref pool.Add(e);
                resourceComponent.Transform = instancePrefab.transform;
                resourceComponent.IsTaken = false;

                _sessionData.ResourceSpawnTimer = _sessionData.ResourceSpawnInterval;
            }
        }
        
        private bool TryGetSpawnPoint(Collider zone, out Vector3 point)
        {
            Bounds bounds = zone.bounds;

            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPoint = new(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y + 200f, Random.Range(bounds.min.z, bounds.max.z));

                if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 400f, _resourcesConfig.LayerMask))
                {
                    if (zone.bounds.Contains(hit.point))
                    {
                        point = hit.point + Vector3.up * 0.3f;
                        return true;
                    }
                }
            }

            point = Vector3.zero;
            return false;
        }
        
        private void ChangeSpawnInterval()
        {
            _sessionData.ResourceSpawnTimer = _sessionData.ResourceSpawnInterval;
        }

        public void Destroy(IEcsSystems systems)
        {
           _gameEvents.OnDronesSpeedChanged -= ChangeSpawnInterval;
        }
    }
}
