using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BuildingManager : MonoCash
{
    [Inject] private DataManager DataManager;
    [Inject] private BlockCreater BlockCreater;
    [Inject] private Helper Helper;

    [Header("Hook")]
    [SerializeField] private float hookRadiusY = 0.25f;
    [SerializeField] private float hookRadiusX = 1f;
    [SerializeField] private float hookMovementSpeed = 2f;
    [SerializeField] private float hookIntervalIncreaseSpeed = 0.05f;
    [SerializeField] private Transform hookTransform;
    private float hookAngleMovement;
    private Vector3 hookCenterPos;
    private Vector3 hookStartPos;

    [Header("Block")]
    [SerializeField] public float AttachmentDelay = 2;
    [SerializeField] private float offsetSpawnY = 1;
    [SerializeField] private float cooldownNewBlock = 0.5f;
    private bool isNewBlockReady;
    private IBlock currentBlock;

    [Header("Shake Block Animation")]
    [SerializeField] private int blockVibratoShake = 1;
    [SerializeField] private float blockDurationShake = 0.2f;
    [SerializeField] private Vector3 blockStrengthShake;


    public int HeightBuilding { get; private set; }
    private Queue<IBlock> tower = new();
    private Tweener tweener;

    public override void OnTick()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ThrowBlock();

        HookMovementCircle();
    }

    public void Initialize()
    {
        BlockCreater.Initialize();
        CreateNewBlock();

        hookCenterPos = hookStartPos = hookTransform.position;
    }

    public void ThrowBlock()
    {
        if (isNewBlockReady)
        {
            hookTransform.DetachChildren();
            
            currentBlock.Fall();

            currentBlock = null;
            isNewBlockReady = false;

            Helper.ExecuteWithDelay(cooldownNewBlock, () => CreateNewBlock());
        }
    }

    private void CreateNewBlock()
    {
        IBlock buildingBlock = BlockCreater.Get(hookTransform.position + Vector3.down * offsetSpawnY);
        buildingBlock.SetParent(hookTransform);

        currentBlock = buildingBlock;
        
        Transform blockTransform = buildingBlock.GetTransform();
        Vector3 initialScale = blockTransform.localScale;
        blockTransform.localScale = Vector3.one * 0.1f;
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(blockTransform.DOScale(initialScale, 0.15f))
            .Append(blockTransform.DOPunchScale(blockStrengthShake, blockDurationShake, blockVibratoShake, 0))
            .OnComplete(() => isNewBlockReady = true);
    }

    private void HookMovementCircle()
    {
        hookAngleMovement += hookMovementSpeed * Time.deltaTime;
        float x = hookCenterPos.x + hookRadiusX * Mathf.Cos(hookAngleMovement);
        float y = hookCenterPos.y + hookRadiusY * Mathf.Sin(hookAngleMovement);

        hookTransform.position = new Vector2(x, y);
    }

    /*private void Move()
    {
        float height = GameData.HeightBuildingBlock;
        int heightBuilding = buildingManager.HeightBuilding;
        int skipBlocksForMovement = gameData.SkipBlocksForMovement;

        if (heightBuilding < skipBlocksForMovement) return;
        tweener.Kill();

        tweener = DOTween.To(
            () => centerPosition,
            x => centerPosition = x,
            new Vector3(centerPosition.x, startPos.y + (heightBuilding - skipBlocksForMovement) * height),
            1);

        Debug.Log(heightBuilding);
        IncreaseSpeed();
    }*/

    public void AddBuildingBlock(IBlock buildingBlock)
    {
        tower.Enqueue(buildingBlock);
        
        HeightBuilding++;

        if (HeightBuilding == 2)
        {
            tower.Peek().Strengthen();
            tower.ElementAt(1).SetBreakTorque();
        }
        else if (HeightBuilding % 10 == 0) CleanTower(5);
    }

    //private void IncreaseSpeed() => speed += intervalIncreaseSpeed;

    private void CleanTower(int count)
    {
        for (int i = 0; i < count; i++)
        {
            IBlock buildingBlock = tower.Dequeue();
            buildingBlock.Deactivate();
        }

        tower.Peek().Strengthen();
        tower.ElementAt(1).SetBreakTorque();
    }
}
