using UnityEngine;
using Zenject;


public class AccountManager : MonoBehaviour
{
    [Inject] private DataManager DataManager;

    public LoaderAsset LoaderAsset { get; private set; }

    public DataSkinAsset DataSkinAsset { get; private set; }

    private void Awake()
    {
#if UNITY_EDITOR
        DataSkinAsset = new();
        LoaderAsset = DataManager.LoaderAssets[0];
#endif   
    }
}
