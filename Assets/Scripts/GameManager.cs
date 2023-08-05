using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;

    public static bool IsPause { get; private set; }

    private void Start()
    {
        viewMenu.Open();
    }

    public static void Pause() => IsPause = true;

    public static void Play() => IsPause = false;
}