using UnityEngine;
using Zenject;

public class PopupPause : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;
    [Inject] private ViewGame viewGame;

    [SerializeField] private GameObject container;

    public void Open()
    {
        GlobalUpdate.Pause();
        container.SetActive(true);
    }

    public void Close()
    {
        GlobalUpdate.Play();
        container.SetActive(false);
    }

    public void BtnMenu()
    {
        viewGame.Close();
        viewMenu.Open();
        Close();
    }
}
