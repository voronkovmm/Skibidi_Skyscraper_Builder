using UnityEngine;

public abstract class Citizen : MonoBehaviour
{
    protected CitizenCreator pool;
    protected Camera cameraMain;

    public abstract void Activate();

    public void Deactivate()
    {
        gameObject.SetActive(false);
        pool.Return(this);
    }

    public void Initialize(CitizenCreator pool)
    {
        this.pool = pool;
        cameraMain = Camera.main;
    }
}
