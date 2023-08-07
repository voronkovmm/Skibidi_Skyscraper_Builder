using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Helper
{
    [Inject] private GameManager gameManager;

    public void ExecuteWithDelay(float delay, Action action) => gameManager.StartCoroutine(CoroutineExecuteWithDelay(delay, action));

    private IEnumerator CoroutineExecuteWithDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }
}