using UnityEngine;

[CreateAssetMenu(fileName = "New Loader Asset", menuName = "My Assets/Loader Asset")]
public class LoaderAsset : ScriptableObject
{
    public BlockAsset[] blocks;
    public BuildingBlock blockPrefab;
    public PopupText popupTextPrefab;
}