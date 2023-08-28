using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CitizenCreator
{
    [Inject] private DiContainer Container;
    [Inject] private AccountManager AccountManager;

    private LoaderAsset loaderAsset;

    private Queue<Citizen> pool;
    private List<Citizen> destroyedList;

    public Citizen Get()
    {
        if (pool == null) CreatePool();

        if (pool.Count == 0) pool.Enqueue(Create());

        Citizen instance = pool.Dequeue();
        instance.Activate();
        return instance;
    }

    public void Return(Citizen item) => pool.Enqueue(item);

    public void DestroyAll()
    {
        destroyedList.ForEach(_ => GameObject.Destroy(_.gameObject));

        destroyedList.Clear();
        pool.Clear();
    }

    private Citizen Create()
    {
        Citizen instance = Container.InstantiatePrefab(loaderAsset.PrefabCitizen).GetComponent<Citizen>();
        instance.Initialize(this);
        destroyedList.Add(instance);
        return instance;
    }

    private void CreatePool()
    {
        loaderAsset = AccountManager.LoaderAsset;

        destroyedList = new();
        pool = new();
    }
}   