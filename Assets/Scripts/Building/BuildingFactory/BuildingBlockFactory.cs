using UnityEngine;

public class BuildingBlockFactory
{
    private BlockPool blockPool;

    public BuildingBlockFactory(LoaderAsset loaderAsset, BuildingBlock.Factory factory) => blockPool = new BlockPool(loaderAsset, factory);

    public BuildingBlock NewBlock(Vector3 position) => blockPool.Get(position);
}
