using System.Collections.Generic;
using UnityEngine;

public class MonoCash : MonoBehaviour
{
    public static List<MonoCash> AllUpdate = new List<MonoCash>(50);

    private void OnEnable() => AllUpdate.Add(this);
    private void OnDisable() => AllUpdate.Remove(this);
    private void OnDestroy() => AllUpdate.Remove(this);

    public void Tick() => OnTick();
    public virtual void OnTick() { }
}