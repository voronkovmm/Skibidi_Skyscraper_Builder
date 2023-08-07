using UnityEngine;
using Zenject;

public class PopupGameOver : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;

    [SerializeField] private GameObject container;

    public void Open()
    {
        container.SetActive(true);
    }

    public void BtnMenu()
    {
        container.SetActive(false);
        viewMenu.Open();
    }
}