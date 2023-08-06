using UnityEngine;
using Zenject;

public class GameData
{
    [Inject] private DataManager dataManager;
    private LoaderAsset loaderAsset;

    public static float HeightBuildingBlock { get; private set; }
    public static float WidthBuildingBlock { get; private set; }
    public readonly int SkipBlocksForMovement = 1;

    public void Initiailize()
    {
        loaderAsset = dataManager.GetLoaderAsset();
        SpriteRenderer buildingBlockSpriteRenderer = loaderAsset.blockPrefab.GetComponent<SpriteRenderer>();
        HeightBuildingBlock = buildingBlockSpriteRenderer.bounds.size.y;
        WidthBuildingBlock = buildingBlockSpriteRenderer.bounds.size.x;
    }
}
