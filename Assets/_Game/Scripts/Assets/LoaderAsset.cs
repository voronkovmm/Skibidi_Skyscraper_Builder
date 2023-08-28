using UnityEngine;

[CreateAssetMenu(fileName = "New Loader Asset", menuName = "My Assets/Loader Asset")]
public class LoaderAsset : ScriptableObject
{
    public BlockAsset[] BlockAssets;
    public Block BlockPrefab;
    public Citizen PrefabCitizen;
    public PopupText PopupTextPrefab;
}