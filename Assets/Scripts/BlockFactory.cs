using System.Collections.Generic;
using UnityEngine;

public class BlockFactory
{
    private LoaderAsset loaderAsset;
    private Dictionary<int, Queue<BuildingBlock>> pool;

    public BlockFactory(LoaderAsset loaderAsset)
    {
        this.loaderAsset = loaderAsset;

        CreatePool();
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
        BuildingBlock instance = Object.Instantiate(loaderAsset.blockPrefab);
        instance.Initialize(this, indexBlock, loaderAsset.blocks[indexBlock].sprite);
        return instance;
    }

    private void CreatePool()
    {
        pool = new(loaderAsset.blocks.Length);

        for (int i = 0; i < loaderAsset.blocks.Length; i++)
        {
            pool.Add(i, new Queue<BuildingBlock>());
        }
    }
}
