﻿using UnityEngine;

public interface IBlock
{
    public void Activate(Vector3 position);

    public void Deactivate();

    public void Initialize(BlockCreater pool, int poolIndex, Sprite sprite);

    public void SetParent(Transform parent);

    public void Fall();

    public void Strengthen();

    public void SetBreakTorque();

    public Transform GetTransform();
}