using Leopotam.EcsLite;
using Source.Code.Data;
using Source.Code.ECS.Components;
using Source.Code.MonoBehaviours.Starships.Behaviours;
using UnityEngine;
using DG.Tweening;
using Source.Code.Enums;
using Source.Code.MonoBehaviours;
using Source.Code.MonoBehaviours.UI;
using UnityEngine.AI;
using Zenject;

namespace Source.Code.ECS.Systems
{
    public class StarshipsMoveSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        [Inject] private SessionData _sessionData;
        [Inject] private GameEvents _gameEvents;
        [Inject] private GameUI _gameUI;

        private EcsFilter _starshipFilter;
        private EcsFilter _resourceFilter;
        private EcsFilter _locationFilter;

        private EcsPool<StarshipComponent> _starshipPool;
        private EcsPool<ResourceComponent> _resourcePool;
        private EcsPool<LocationComponent> _locationPool;

        private LocationComponent _locationComponent;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _starshipFilter = world.Filter<StarshipComponent>().End();
            _resourceFilter = world.Filter<ResourceComponent>().End();
            _locationFilter = world.Filter<LocationComponent>().End();

            _starshipPool = world.GetPool<StarshipComponent>();
            _resourcePool = world.GetPool<ResourceComponent>();
            _locationPool = world.GetPool<LocationComponent>();

            foreach (int i in _locationFilter)
            {
                _locationComponent = _locationPool.Get(i);
            }

            ChangeSpeed();

            _gameEvents.OnDronesSpeedChanged += ChangeSpeed;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int i in _starshipFilter)
            {
                ref StarshipComponent starshipComponent = ref _starshipPool.Get(i);
                StarshipBehaviour starshipBehaviour = starshipComponent.StarshipBehaviour;

                starshipComponent.StateTimer -= Time.deltaTime;

                bool isActive = IsActiveShip(starshipComponent);

                if (!isActive && starshipComponent.State != StarshipState.IdleOnPad)
                {
                    if (starshipComponent.CarryingResourceEntity >= 0)
                    {
                        if (starshipComponent.State == StarshipState.CollectingResource ||
                            starshipComponent.State == StarshipState.ReturningWithResource)
                        {
                            if (starshipComponent.State != StarshipState.ReturningWithResource)
                            {
                                LaunchingPad launchingPad = GetPadForShip(starshipComponent);
                                starshipBehaviour.SetPath(launchingPad.SpawnPoint.position, false);
                                starshipComponent.State = StarshipState.ReturningWithResource;
                            }
                        }
                        
                        else
                        {
                            if (_resourcePool.Has(starshipComponent.CarryingResourceEntity))
                            {
                                ref ResourceComponent resourceComponent = ref _resourcePool.Get(starshipComponent.CarryingResourceEntity);
                                resourceComponent.IsTaken = false;
                            }

                            starshipComponent.CarryingResourceEntity = -1;
                            starshipComponent.State = StarshipState.Landing;
                            starshipComponent.StateTimer = 0f;
                        }
                    }
                    else
                    {
                        if (starshipComponent.State != StarshipState.Landing)
                        {
                            NavMeshAgent starshipAgent = starshipBehaviour.NavMeshAgent;
                            LaunchingPad landingPad = GetPadForShip(starshipComponent);

                            if (starshipAgent.enabled && starshipAgent.isOnNavMesh)
                            {
                                starshipAgent.ResetPath();
                            }

                            float landingThreshold = Mathf.Max(starshipAgent.stoppingDistance, starshipAgent.radius) + 0.5f;
                            float sqrDistance = (starshipAgent.transform.position - landingPad.SpawnPoint.position).sqrMagnitude;

                            if (sqrDistance <= landingThreshold * landingThreshold)
                            {
                                starshipBehaviour.Landing();
                                starshipComponent.State = StarshipState.Landing;
                                starshipComponent.StateTimer = 1f;
                            }
                            
                            else
                            {
                                starshipBehaviour.SetPath(landingPad.SpawnPoint.position, false);
                                starshipComponent.State = StarshipState.Landing;
                                starshipComponent.StateTimer = 0f;
                            }
                        }
                    }
                }

                switch (starshipComponent.State)
                {
                    case StarshipState.IdleOnPad:
                        if (isActive)
                        {
                            starshipBehaviour.TakeOff();
                            starshipComponent.State = StarshipState.TakingOff;
                            starshipComponent.StateTimer = 1f;
                        }

                        break;

                    case StarshipState.TakingOff:
                        if (!isActive)
                        {
                            starshipComponent.State = StarshipState.Landing;
                            starshipComponent.StateTimer = 0f;

                            break;
                        }

                        if (starshipComponent.StateTimer <= 0)
                        {
                            int resEntity = FindClosestFreeResource(starshipBehaviour.transform.position);

                            if (resEntity >= 0)
                            {
                                ref ResourceComponent resourceComponent = ref _resourcePool.Get(resEntity);
                                resourceComponent.IsTaken = true;
                                starshipComponent.CarryingResourceEntity = resEntity;

                                starshipBehaviour.SetPath(resourceComponent.Transform.position, true);
                                starshipComponent.State = StarshipState.FlyingToResource;
                            }

                            else
                            {
                                if (starshipBehaviour.NavMeshAgent.enabled && starshipBehaviour.NavMeshAgent.isOnNavMesh)
                                {
                                    starshipBehaviour.NavMeshAgent.ResetPath();
                                }
                            }
                        }

                        break;

                    case StarshipState.FlyingToResource:
                        if (!isActive)
                        {
                            starshipComponent.State = StarshipState.Landing;
                            starshipComponent.StateTimer = 0f;

                            break;
                        }

                        if (!starshipBehaviour.NavMeshAgent.pathPending && starshipBehaviour.NavMeshAgent.remainingDistance < 0.5f)
                        {
                            starshipComponent.State = StarshipState.CollectingResource;
                            starshipComponent.StateTimer = 2f;

                            if (starshipComponent.CarryingResourceEntity >= 0 && _resourcePool.Has(starshipComponent.CarryingResourceEntity))
                            {
                                ref ResourceComponent resourceComponent = ref _resourcePool.Get(starshipComponent.CarryingResourceEntity);
                                resourceComponent.IsTaken = true;

                                resourceComponent.Transform.DOKill();
                                resourceComponent.Transform.DOMove(starshipComponent.TrunkTransform.position, 2f).SetEase(Ease.OutCubic);
                            }
                            else
                            {
                                starshipComponent.CarryingResourceEntity = -1;
                            }
                        }

                        break;

                    case StarshipState.CollectingResource:
                        if (starshipComponent.StateTimer <= 0)
                        {
                            LaunchingPad launchingPad = GetPadForShip(starshipComponent);

                            starshipComponent.State = StarshipState.ReturningWithResource;
                            starshipBehaviour.SetPath(launchingPad.SpawnPoint.position, false);
                        }

                        break;

                    case StarshipState.ReturningWithResource:
                    {
                        NavMeshAgent starshipAgent = starshipBehaviour.NavMeshAgent;
                        float arriveDistance = Mathf.Max(starshipAgent.stoppingDistance, starshipAgent.radius) + 0.5f;

                        if (starshipAgent.pathPending == false && starshipAgent.remainingDistance <= arriveDistance)
                        {
                            if (starshipComponent.StateTimer <= 0)
                            {
                                starshipComponent.StateTimer = 2f;
                            }

                            if (starshipComponent.CarryingResourceEntity >= 0 && _resourcePool.Has(starshipComponent.CarryingResourceEntity))
                            {
                                ref ResourceComponent resourceComponent = ref _resourcePool.Get(starshipComponent.CarryingResourceEntity);
                                Transform resTransform = resourceComponent.Transform;
                                LaunchingPad pad = GetPadForShip(starshipComponent);

                                resTransform.position = Vector3.MoveTowards(
                                    resTransform.position,
                                    pad.SpawnPoint.position,
                                    Time.deltaTime * 10f
                                );
                            }
                            else
                            {
                                starshipComponent.CarryingResourceEntity = -1;
                            }

                            starshipComponent.StateTimer -= Time.deltaTime;

                            if (starshipComponent.StateTimer <= 0 && starshipComponent.CarryingResourceEntity >= 0 &&
                                _resourcePool.Has(starshipComponent.CarryingResourceEntity))
                            {
                                int resEntity = starshipComponent.CarryingResourceEntity;
                                ref ResourceComponent res = ref _resourcePool.Get(resEntity);

                                if (starshipComponent.Faction == FractionType.Gray)
                                {
                                    _sessionData.GrayTeamScore += 1;
                                    _gameUI.GameScreenUI.GrayTeamScore.SetScore(_sessionData.GrayTeamScore);
                                }

                                else
                                {
                                    _sessionData.RedTeamScore += 1;
                                    _gameUI.GameScreenUI.RedTeamScore.SetScore(_sessionData.GrayTeamScore);
                                }

                                Object.Destroy(res.Transform.gameObject);
                                _resourcePool.Del(resEntity);
                                starshipComponent.CarryingResourceEntity = -1;

                                if (!IsActiveShip(starshipComponent))
                                {
                                    starshipBehaviour.Landing();
                                    starshipComponent.State = StarshipState.Landing;
                                    starshipComponent.StateTimer = 1f;
                                }

                                else
                                {
                                    starshipComponent.State = StarshipState.IdleOnPad;
                                }
                            }
                        }

                        else
                        {
                            if (starshipComponent.CarryingResourceEntity >= 0 && _resourcePool.Has(starshipComponent.CarryingResourceEntity))
                            {
                                ref ResourceComponent resourceComponent = ref _resourcePool.Get(starshipComponent.CarryingResourceEntity);
                                resourceComponent.Transform.position = starshipComponent.TrunkTransform.position;
                            }
                            else
                            {
                                starshipComponent.CarryingResourceEntity = -1;
                            }
                        }

                        break;
                    }

                    case StarshipState.Landing:
                    {
                        LaunchingPad launchingPad = GetPadForShip(starshipComponent);
                        NavMeshAgent starshipAgent = starshipBehaviour.NavMeshAgent;

                        if (starshipAgent.enabled && starshipAgent.isOnNavMesh)
                        {
                            float landingThreshold = Mathf.Max(starshipAgent.stoppingDistance, starshipAgent.radius) + 0.5f;
                            float sqrDist = (starshipAgent.transform.position - launchingPad.SpawnPoint.position).sqrMagnitude;

                            if (sqrDist <= landingThreshold * landingThreshold)
                            {
                                if (starshipComponent.StateTimer <= 0)
                                {
                                    starshipBehaviour.Landing();
                                    starshipComponent.StateTimer = 1f;
                                }
                            }
                            
                            else
                            {
                                if (starshipAgent.pathPending || !starshipAgent.hasPath || starshipAgent.remainingDistance > landingThreshold)
                                {
                                    starshipBehaviour.SetPath(launchingPad.SpawnPoint.position, false);
                                }
                            }
                        }
                        
                        else
                        {
                            if (starshipComponent.StateTimer <= 0)
                            {
                                starshipComponent.State = StarshipState.IdleOnPad;
                                starshipComponent.CarryingResourceEntity = -1;
                            }
                        }

                        break;
                    }
                }

                bool showPath = IsActiveShip(starshipComponent) && _sessionData.IsHowStarshipsPath && starshipBehaviour.NavMeshAgent.hasPath;
                starshipBehaviour.DrawPath(showPath);
            }
        }

        private bool IsActiveShip(StarshipComponent starshipComponent)
        {
            return starshipComponent.StarshipBehaviour.LaunchingPadIndex < _sessionData.ActiveShipsCount;
        }

        private int FindClosestFreeResource(Vector3 from)
        {
            float minDistance = float.MaxValue;
            int closest = -1;

            foreach (int e in _resourceFilter)
            {
                ref ResourceComponent resourceComponent = ref _resourcePool.Get(e);

                if (resourceComponent.IsTaken || resourceComponent.Transform == null)
                    continue;

                float distance = Vector3.SqrMagnitude(from - resourceComponent.Transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = e;
                }
            }

            return closest;
        }

        private LaunchingPad GetPadForShip(StarshipComponent starshipComponent)
        {
            return starshipComponent.Faction == FractionType.Gray
                ? _locationComponent.GrayTeamLaunchingPads[starshipComponent.StarshipBehaviour.LaunchingPadIndex]
                : _locationComponent.RedTeamLaunchingPads[starshipComponent.StarshipBehaviour.LaunchingPadIndex];
        }

        private void ChangeSpeed()
        {
            foreach (int i in _starshipFilter)
            {
                ref StarshipComponent starshipComponent = ref _starshipPool.Get(i);
                StarshipBehaviour starshipBehaviour = starshipComponent.StarshipBehaviour;

                starshipBehaviour.NavMeshAgent.speed = _sessionData.ShipSpeed;
            }
        }

        public void Destroy(IEcsSystems systems)
        {
            _gameEvents.OnDronesSpeedChanged -= ChangeSpeed;
        }
    }
}