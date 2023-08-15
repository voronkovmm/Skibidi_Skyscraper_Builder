using Zenject;

public class Installer : MonoInstaller
{
    public int indexLoaderAsset;

    public override void InstallBindings()
    {
        Container.Bind<AccountManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<DataManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BuildingManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<ViewMenu>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ViewGame>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<PopupSettings>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupLeaderboard>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupAchivements>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupPause>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupChouseBlockAsset>().FromComponentInHierarchy().AsSingle();

        Container.Bind<CameraMovement>().FromComponentInHierarchy().AsSingle();

        Container.Bind<BlockCreater>().FromNew().AsSingle();

        BindPopupTextService();
    }

    private void BindPopupTextService()
    {
        /*PopupText popupTextPrefab = loaderAssets[indexLoaderAsset].popupTextPrefab;

        Container.Bind<PopupTextService>()
            .FromNew()
            .AsSingle()
            .WithArguments(popupTextPrefab)
            .NonLazy();*/
    }
}
