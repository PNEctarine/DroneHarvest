using Source.Code.Data;
using Source.Code.MonoBehaviours.UI;
using Source.Code.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Source.Code.Zenject
{
    public class LocationContextInstaller : MonoInstaller
    {
        [SerializeField] private StarshipsConfig _starshipsConfig;
        [SerializeField] private ResourcesConfig _resourcesConfig;
        
        [Space(10)]
        [SerializeField] private GameUI _gameUI;

        public override void InstallBindings()
        {
            Container.Bind<StarshipsConfig>().FromInstance(_starshipsConfig).AsSingle().NonLazy();
            Container.Bind<ResourcesConfig>().FromInstance(_resourcesConfig).AsSingle().NonLazy();

            Container.Bind<GameUI>().FromComponentInNewPrefab(_gameUI).AsSingle().NonLazy();
            
            Container.Bind<SessionData>().FromNew().AsSingle().NonLazy();
            Container.Bind<GameEvents>().FromNew().AsSingle().NonLazy();
        }
    }
}
