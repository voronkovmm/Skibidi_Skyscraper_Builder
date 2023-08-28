using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ViewGame : MonoBehaviour
{
    [Inject] private PopupPause popupPause;
    [Inject] public BuildingManager BuildingManager;
    [Inject] private WidgetSliderScore WidgetSliderScore;

    [SerializeField] private GameObject container;
    [SerializeField] private WidgetFixation WidgetFixation;

    [HideInInspector] public bool IsGame;

    public void BtnPause() => popupPause.Open();

    public void Open()
    {
        IsGame = true;
        BuildingManager.Initialize();
        WidgetSliderScore.Initiailize();
        WidgetFixation.Initialize(this);
        container.SetActive(true);
    }

    public void OnClickFixation() => WidgetFixation.OnClick();

    public void Close()
    {
        IsGame = false;
        BuildingManager.OnExitMenu();
        container.SetActive(false);
    }
}