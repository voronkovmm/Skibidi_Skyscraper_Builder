using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;
    [Inject] private HookManager hookManager;
    [Inject] private GameData gameData;

    public static bool IsPause { get; private set; }

    private void Start()
    {
        viewMenu.Open();
    }

    public static void Pause() => IsPause = true;

    public static void Play() => IsPause = false;

    public void StartGame()
    {
        hookManager.Initiailize();
        gameData.Initiailize();
    }
}