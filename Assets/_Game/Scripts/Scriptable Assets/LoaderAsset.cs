using UnityEngine;

[CreateAssetMenu(fileName = "New Loader Asset", menuName = "My Assets/Loader Asset")]
public class LoaderAsset : ScriptableObject
{
    public BlockAsset[] blocks;
    public Block blockPrefab;
    public PopupText popupTextPrefab;
}