using UnityEngine;
using Zenject;

public class ViewGame : MonoBehaviour
{
    [Inject] private PopupPause popupPause;
    [Inject] private BuildingManager buildingManager;

    [SerializeField] private GameObject container;

    public bool IsGame;

    public void BtnPause() => popupPause.Open();

    public void Open()
    {
        IsGame = true;
        buildingManager.Initialize();
        container.SetActive(true);
    }

    public void Close()
    {
        IsGame = false;
        buildingManager.OnExitMenu();
        container.SetActive(false);
    }
}