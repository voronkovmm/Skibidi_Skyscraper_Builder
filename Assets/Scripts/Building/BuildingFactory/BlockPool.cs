using System.Collections.Generic;
using UnityEngine;


public class BlockPool
{
    private LoaderAsset loaderAsset;
    private BuildingBlock.Factory factory;
    private Dictionary<int, Queue<BuildingBlock>> pool;

    public BlockPool(LoaderAsset loaderAsset, BuildingBlock.Factory factory)
    {
        this.loaderAsset = loaderAsset;
        this.factory = factory;
        pool = new(loaderAsset.blocks.Length);
        for (int i = 0; i < loaderAsset.blocks.Length; i++)
        {
            pool.Add(i, new Queue<BuildingBlock>());
        }
    }

    public BuildingBlock Get(Vector3 position)
    {
        int blockIndex = Random.Range(0, pool.Count);

        if (pool[blockIndex].Count == 0)
        {
            pool[blockIndex].Enqueue(Create(blockIndex));
        }

        BuildingBlock instance = pool[blockIndex].Dequeue();
        instance.Activate(position);
        return instance;
    }

    public void Return(BuildingBlock buildingBlock, int index) => pool[index].Enqueue(buildingBlock);

    private BuildingBlock Create(int indexBlock)
    {
        //BuildingBlock instance = Object.Instantiate(loaderAsset.blockPrefab);
        BuildingBlock instance = factory.Create();
        instance.Initialize(this, indexBlock, loaderAsset.blocks[indexBlock].sprite);
        return instance;
    }
}
