using UnityEngine;

public class GlobalUpdate : MonoBehaviour
{
    private void Update()
    {
        for (int i = 0; i < MonoCash.AllUpdate.Count; i++) 
            MonoCash.AllUpdate[i].Tick();
    }
}