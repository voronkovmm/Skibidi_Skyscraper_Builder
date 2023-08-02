using UnityEngine;

public class GameData
{
    private LoaderAsset loaderAsset;

    public readonly float HeightBuildingBlock;
    public readonly float WidthBuildingBlock;
    public readonly int SkipBlocksForMovement = 1;

    public GameData(LoaderAsset loaderAsset)
    {
        this.loaderAsset = loaderAsset;
        SpriteRenderer buildingBlockSpriteRenderer = loaderAsset.blockPrefab.GetComponent<SpriteRenderer>();
        HeightBuildingBlock = buildingBlockSpriteRenderer.bounds.size.y;
        WidthBuildingBlock = buildingBlockSpriteRenderer.bounds.size.x;
    }
}
