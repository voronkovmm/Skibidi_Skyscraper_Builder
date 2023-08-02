using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private LoaderAsset[] loaderAssets;
    private int indexLoaderAsset;

    public override void InstallBindings()
    {
        indexLoaderAsset = 0;
        BindGameData();
        BindPopupTextService();
        BindBuildingBlockFactory();
        BindBuildingManager();
    }

    private void BindBuildingManager()
    {
        Container.Bind<BuildingManager>()
                    .FromInstance(buildingManager)
                    .AsSingle()
                    .NonLazy();
    }

    private void BindPopupTextService()
    {
        PopupText popupTextPrefab = loaderAssets[indexLoaderAsset].popupTextPrefab;

        Container.Bind<PopupTextService>()
            .FromNew()
            .AsSingle()
            .WithArguments(popupTextPrefab)
            .NonLazy();
    }

    private void BindGameData()
    {
        Container.Bind<GameData>()
                    .FromNew()
                    .AsSingle()
                    .WithArguments(loaderAssets[indexLoaderAsset])
                    .NonLazy();
    }

    private void BindBuildingBlockFactory()
    {
        Container.Bind<BuildingBlockFactory>()
            .FromNew()
            .AsSingle()
            .WithArguments(loaderAssets[indexLoaderAsset])
            .NonLazy();

        Container.BindFactory<BuildingBlock, BuildingBlock.Factory>().FromComponentInNewPrefab(loaderAssets[indexLoaderAsset].blockPrefab);
    }
}
