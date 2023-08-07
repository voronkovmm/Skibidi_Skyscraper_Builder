using Zenject;

public class Installer : MonoInstaller
{
    public int indexLoaderAsset;

    public override void InstallBindings()
    {
        Container.Bind<AccountManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DataManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BuildingManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DataGame>().FromNew().AsSingle();

        Container.Bind<ViewMenu>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ViewGame>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<PopupSettings>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupLeaderboard>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupAchivements>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupPause>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupChouseBlockAsset>().FromComponentInHierarchy().AsSingle();

        BindPopupTextService();

        Container.Bind<BlockCreater>().FromNew().AsSingle();

        Container.Bind<Helper>().FromNew().AsSingle();
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
