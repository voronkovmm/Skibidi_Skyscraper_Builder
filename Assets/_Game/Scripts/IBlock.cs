using UnityEngine;

public interface IBlock
{
    public GameObject GameObject { get; }

    public void Activate(Vector3 position);

    public void Deactivate();

    public void Initialize(BlockCreator pool, int poolIndex, Sprite sprite);

    public void SetParent(Transform parent);

    public void Fall();

    public void Strengthen();

    public void SetBreakTorque();

    public Transform GetTransform();

    public void Fixation();
}