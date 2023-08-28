using DG.Tweening;
using UnityEngine;
using Zenject;

public class CameraMovement : MonoBehaviour
{
    [Inject] private BuildingManager BuildingManager;

    private Vector3 startPos;
    private Tweener tweenerMovement;

    private void Awake() => startPos = transform.position;

    public void Move()
    {
        tweenerMovement.Kill();
        tweenerMovement = transform.DOMoveY(startPos.y + (BuildingManager.TotalHeightBuilding - BuildingManager.SkipBlocksForMovement) * BuildingManager.BlockHeight, 1f);
    }

    public void Restart()
    {
        tweenerMovement.Kill();
        transform.position = startPos;
    }
}