using UnityEngine;
using Zenject;

public class PopupTextService
{
    private PopupTextPool popupPool;

    public PopupTextService(PopupText prefab) => popupPool = new PopupTextPool(prefab);

    public void Show(Vector3 position, string text, Color color) => popupPool.Get(position, text, color);
}
