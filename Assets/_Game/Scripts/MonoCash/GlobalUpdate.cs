using UnityEngine;

public class GlobalUpdate : MonoBehaviour
{
    public static bool IsPause { get; private set; }

    private void Update()
    {
        if (IsPause) return;

        for (int i = 0; i < MonoCash.AllUpdate.Count; i++) 
            MonoCash.AllUpdate[i].Tick();
    }

    public static void Pause() => IsPause = true;

    public static void Play() => IsPause = false;
}