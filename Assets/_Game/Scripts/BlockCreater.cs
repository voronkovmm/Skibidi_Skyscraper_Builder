using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BlockCreater
{
    [Inject] private DiContainer Container;
    [Inject] private DataManager DataManager;

    private LoaderAsset loaderAsset;

    private Dictionary<int, Queue<IBlock>> pool;

    public void Initialize() => CreatePool();

    public IBlock Get(Vector3 position)
    {
        int blockIndex = Random.Range(0, pool.Count);

        if (pool[blockIndex].Count == 0)
        {
            pool[blockIndex].Enqueue(Create(blockIndex));
        }

        IBlock instance = pool[blockIndex].Dequeue();
        instance.Activate(position);
        return instance;
    }

    public void Return(IBlock buildingBlock, int index) => pool[index].Enqueue(buildingBlock);

    private IBlock Create(int indexBlock)
    {
        IBlock instance = Container.InstantiatePrefab(loaderAsset.blockPrefab.gameObject).GetComponent<IBlock>();
        instance.Initialize(this, indexBlock, loaderAsset.blocks[indexBlock].sprite);
        return instance;
    }

    private void CreatePool()
    {
        loaderAsset = DataManager.GetLoaderAsset();

        pool = new(loaderAsset.blocks.Length);

        for (int i = 0; i < loaderAsset.blocks.Length; i++)
        {
            pool.Add(i, new Queue<IBlock>());
        }
    }
}
