using System.Collections.Generic;
using UnityEngine;

public class PopupTextPool
{
    private PopupText prefab;
    private Queue<PopupText> pool = new();
    public PopupTextPool(PopupText prefab)
    {
        this.prefab = prefab;
        InitializePool(1);
    }

    private void InitializePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            pool.Enqueue(Create());
        }
    }

    public PopupText Get(Vector3 position, string text, Color color)
    {
        if(pool.Count == 0)
        {
            pool.Enqueue(Create());
        }

        PopupText instance = pool.Dequeue();
        instance.Activate(position, text, color);
        return instance;
    }

    public void Return(PopupText popupText)
    {
        pool.Enqueue(popupText);
    }

    private PopupText Create()
    {
        PopupText instance = Object.Instantiate(prefab);
        instance.gameObject.SetActive(false);
        instance.Initialize(this);
        return instance;
    }
}
