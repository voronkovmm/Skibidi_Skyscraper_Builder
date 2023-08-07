using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;
    [Inject] private DataGame gameData;
    [Inject] private BuildingManager buildingManager;

    public static bool IsPause { get; private set; }

    private void Start()
    {
        Pause();
        viewMenu.Open();
    }

    public static void Pause() => IsPause = true;

    public static void Play() => IsPause = false;

    public void StartGame()
    {
        gameData.Initiailize();
        buildingManager.Initialize();
        Play();
    }
}