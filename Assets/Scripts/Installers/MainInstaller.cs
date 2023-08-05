using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private LoaderAsset[] loaderAssets;
    public int indexLoaderAsset;

    public override void InstallBindings()
    {
        BindData();
        BindGameData();
        BindPopupTextService();
        BindBuildingBlockFactory();
        BindBuildingManager();
        BindPopup();
        BindView();
    }

    private void BindData()
    {
        Container.Bind<AccountManager>().FromComponentInHierarchy().AsSingle();
    }

    private void BindPopup()
    {
        Container.Bind<PopupSettings>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupLeaderboard>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupAchivements>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupPause>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupChouseBlockAsset>().FromComponentInHierarchy().AsSingle();
    }

    private void BindView()
    {
        Container.Bind<ViewMenu>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ViewGame>().FromComponentInHierarchy().AsSingle();
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
