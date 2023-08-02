using DG.Tweening;
using UnityEngine;
using Zenject;

public class CameraMovement : MonoBehaviour
{
    private GameData gameData;
    private BuildingManager buildingManager;
    private Tweener tweener;
    private Vector3 startPos;

    [Inject]
    private void Construct(GameData gameData, BuildingManager buildingManager)
    {
        this.gameData = gameData;
        this.buildingManager = buildingManager;
    }
    private void Awake() => startPos = transform.position;

    private void OnEnable() => buildingManager.OnNewBuildingBlock += OnNewBuildingBlock;

    private void OnDisable() => buildingManager.OnNewBuildingBlock -= OnNewBuildingBlock;

    private void Move()
    {
        float height = gameData.HeightBuildingBlock;
        int heightBuilding = buildingManager.HeightBuilding;
        int skipBlocksForMovement = gameData.SkipBlocksForMovement;

        if (heightBuilding < skipBlocksForMovement) return;

        tweener.Kill();
        tweener = transform.DOMoveY(startPos.y + (heightBuilding - skipBlocksForMovement) * height, 1f);
    }

    private void OnNewBuildingBlock() => Move();
}