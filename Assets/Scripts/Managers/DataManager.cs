using UnityEngine;
using Zenject;

public class DataManager : MonoBehaviour
{
    [Inject] private AccountManager accountManager;

    [SerializeField] private LoaderAsset[] loaderAssets;

    public LoaderAsset GetLoaderAsset()
    {
        EnumSkinAsset currentAsset = accountManager.DataSkinAsset.CurrentAsset;

        switch (currentAsset)
        {
            case EnumSkinAsset.BARBIE: return loaderAssets[0];
            case EnumSkinAsset.SKIBIDI: return loaderAssets[1];
            default:
                return loaderAssets[0];
        }
    }
}
