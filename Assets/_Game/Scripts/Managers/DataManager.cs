using UnityEngine;
using Zenject;

public class DataManager : MonoBehaviour
{
    [Inject] private AccountManager accountManager;

    [field: SerializeField] public LoaderAsset[] LoaderAssets { get; private set; }

}
