using UnityEngine;
using Zenject;

public class ViewMenu : MonoBehaviour
{
    [Inject] private PopupSettings popupSettings;
    [Inject] private PopupAchivements popupAchivement;
    [Inject] private PopupLeaderboard popupLeaderboard;
    [Inject] private PopupChouseBlockAsset popupChouseBlockAsset;
    
    [SerializeField] private GameObject container;

    public void Open() => container.SetActive(true);
    public void Close() => container.SetActive(false);

    public void BtnSettings() => popupSettings.Open();
    public void BtnAchivements() => popupAchivement.Open();
    public void BtnLeaderboard() => popupLeaderboard.Open();
    public void BtnPlay() => popupChouseBlockAsset.Open();
}