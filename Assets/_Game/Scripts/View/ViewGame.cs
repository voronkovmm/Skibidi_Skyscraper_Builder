using UnityEngine;
using Zenject;

public class ViewGame : MonoBehaviour
{
    [Inject] private PopupPause popupPause;
    [Inject] private GameManager gameManager;


    [SerializeField] private GameObject container;

    public void BtnPause() => popupPause.Open();

    public void Open()
    {
        gameManager.StartGame();

        container.SetActive(true);
    }

    public void Close()
    {
        container.SetActive(false);
    }
}