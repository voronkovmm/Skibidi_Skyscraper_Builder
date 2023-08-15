using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BuildingManager : MonoCash
{
    [Inject] private AccountManager AccountManager;
    [Inject] private BlockCreater BlockCreater;
    [Inject] private CameraMovement CameraMovement;
    [Inject] private ViewGame ViewGame;

    [Header("Hook")]
    [SerializeField] private float hookRadiusY = 0.25f;
    [SerializeField] private float hookRadiusX = 1f;
    [SerializeField] private float hookMovementSpeed = 2f;
    [SerializeField] private float hookIntervalIncreaseSpeed = 0.05f;
    [SerializeField] private Transform hookTransform;
    private float hookCurrentMovementSpeed;
    private float hookAngleMovement;
    private Vector3 hookCenterPos;
    private Vector3 hookStartPos;
    private Tweener hookTweenerMovement;

    [Header("Block")]
    [SerializeField] public float AttachmentDelay = 2;
    [SerializeField] private float offsetSpawnY = 1;    
    public float BlockHeight { get; private set; }
    public float BlockWidth { get; private set; }
    public readonly int SkipBlocksForMovement = 1;
    private bool isNewBlockReady;
    private IBlock blockCurrent;

    [Header("Shake Block Animation")]
    [SerializeField] private int blockVibratoShake = 1;
    [SerializeField] private float blockDurationShake = 0.2f;
    [SerializeField] private Vector3 blockStrengthShake;


    public int HeightBuilding { get; private set; }
    private Queue<IBlock> tower = new();

    public override void OnTick()
    {
        if (!ViewGame.IsGame) return;

        if (Input.GetKeyDown(KeyCode.F))
            ThrowBlock();

        HookMovementCircle();
    }

    public void Initialize()
    {
        BlockCreater.Initialize();
        HookStart();
        CreateNewBlock();

        hookCenterPos = hookStartPos = hookTransform.position;

        // Высчитываем высоту и ширину блока
        SpriteRenderer buildingBlockSpriteRenderer = AccountManager.LoaderAsset.BlockPrefab.GetComponent<SpriteRenderer>();
        BlockHeight = buildingBlockSpriteRenderer.bounds.size.y;
        BlockWidth = buildingBlockSpriteRenderer.bounds.size.x;
    }

    public void OnExitMenu()
    {
        BlockCreater.DestroyAll();
        CameraMovement.Restart();
        HookRestart();
        TowerRestart();
    }

    public void ThrowBlock()
    {
        if (!isNewBlockReady) return;

        hookTransform.DetachChildren();

        blockCurrent.Fall();

        blockCurrent = null;
        isNewBlockReady = false;
    }

    private void CreateNewBlock()
    {
        IBlock buildingBlock = BlockCreater.Get(hookTransform.position + Vector3.down * offsetSpawnY);
        buildingBlock.SetParent(hookTransform);

        blockCurrent = buildingBlock;
        
        Transform blockTransform = buildingBlock.GetTransform();
        Vector3 initialScale = blockTransform.localScale;
        blockTransform.localScale = Vector3.one * 0.1f;
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(blockTransform.DOScale(initialScale, 0.15f))
            .Append(blockTransform.DOPunchScale(blockStrengthShake, blockDurationShake, blockVibratoShake, 0))
            .OnComplete(() => isNewBlockReady = true);
    }

    #region Hook

    private void HookMovementCircle()
    {
        hookAngleMovement += hookCurrentMovementSpeed * Time.deltaTime;
        float x = hookCenterPos.x + hookRadiusX * Mathf.Cos(hookAngleMovement);
        float y = hookCenterPos.y + hookRadiusY * Mathf.Sin(hookAngleMovement);

        hookTransform.position = new Vector2(x, y);
    }

    private void HookIncreaseSpeed() => hookCurrentMovementSpeed += hookIntervalIncreaseSpeed;

    private void HookStart()
    {
        hookCurrentMovementSpeed = hookMovementSpeed;
    }

    private void HookRestart()
    {
        hookTweenerMovement.Kill();
        hookCurrentMovementSpeed = 0;
        hookTransform.position = hookStartPos;
    }

    #endregion

    #region Tower

    private void MovementUpOrDown()
    {
        if (HeightBuilding < SkipBlocksForMovement) return;

        hookTweenerMovement.Kill();

        hookTweenerMovement = DOTween.To(
            () => hookCenterPos,
            x => hookCenterPos = x,
            new Vector3(hookCenterPos.x, hookStartPos.y + (HeightBuilding - SkipBlocksForMovement) * BlockHeight),
            1);

        CameraMovement.Move();

        HookIncreaseSpeed();
    }

    public void TowerAddBlock(IBlock buildingBlock)
    {
        tower.Enqueue(buildingBlock);
        
        HeightBuilding++;

        if (HeightBuilding == 2)
        {
            tower.Peek().Strengthen();
            tower.ElementAt(1).SetBreakTorque();
        }
        else if (HeightBuilding % 10 == 0) CleanTower(5);

        MovementUpOrDown();

        CreateNewBlock();
    }

    private void TowerRestart()
    {
        HeightBuilding = 0;
        tower.Clear();   
    }

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

    #endregion
}
