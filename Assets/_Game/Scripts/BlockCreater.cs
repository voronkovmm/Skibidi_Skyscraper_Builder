using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BlockCreater
{
    [Inject] private DiContainer Container;
    [Inject] private AccountManager AccountManager;

    private LoaderAsset loaderAsset;

    private Queue<IBlock> pool;
    private List<IBlock> destroyedList;

    public void Initialize() => CreatePool();

    public IBlock Get(Vector3 position)
    {
        int blockIndex = Random.Range(0, pool.Count);

        if (pool.Count == 0)
        {
            pool.Enqueue(Create(blockIndex));
        }

        IBlock instance = pool.Dequeue();
        instance.Activate(position);
        return instance;
    }

    public void Return(IBlock buildingBlock, int index) => pool.Enqueue(buildingBlock);

    public void DestroyAll()
    {
        destroyedList.ForEach(_ => GameObject.Destroy(_.GameObject));

        destroyedList.Clear();
        pool.Clear();
    }

    private IBlock Create(int indexBlock)
    {
        IBlock instance = Container.InstantiatePrefab(loaderAsset.BlockPrefab.gameObject).GetComponent<IBlock>();
        instance.Initialize(this, indexBlock, loaderAsset.BlockAssets[indexBlock].sprite);
        destroyedList.Add(instance);
        instance.GameObject.name = $"block({destroyedList.Count})";
        return instance;
    }

    private void CreatePool()
    {
        loaderAsset = AccountManager.LoaderAsset;

        destroyedList = new();
        pool = new();
    }
}
