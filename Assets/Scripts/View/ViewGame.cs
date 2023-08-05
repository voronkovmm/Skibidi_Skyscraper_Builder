using UnityEngine;
using Zenject;

public class ViewGame : MonoBehaviour
{
    [Inject] private PopupPause popupPause;

    [SerializeField] private GameObject container;

    public void BtnPause() => popupPause.Open();

    public void Open()
    {
        container.SetActive(true);
    }

    public void Close()
    {
        container.SetActive(false);
    }
}