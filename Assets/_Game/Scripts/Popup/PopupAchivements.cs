using UnityEngine;
using Zenject;

public class PopupAchivements : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;

    [SerializeField] private GameObject container;

    public void Open()
    {
        container.SetActive(true);
    }

    public void Close()
    {
        container.SetActive(false);
    }

    public void BtnMenu()
    {
        Close();
        viewMenu.Open();
    }
}
