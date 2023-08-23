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

    [Header("Main")]
    [SerializeField] public int SkipBlocksForMovement = 1;
    [SerializeField] public float CameraOffsetMovement = 9;

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
    public float BlockHalfWidth { get; private set; }
    private bool isNewBlockReady;
    private IBlock blockCurrent;

    [Header("Shake Block Animation")]
    [SerializeField] private int blockVibratoShake = 1;
    [SerializeField] private float blockDurationShake = 0.2f;
    [SerializeField] private Vector3 blockStrengthShake;

    public Vector3 TopBlockPos{ get => tower[^1].GetTransform().position; }


    public int HeightBuilding { get; private set; }
    private List<IBlock> tower = new();

    public override void OnTick()
    {
        if (!ViewGame.IsGame) return;

        if (Input.GetKeyDown(KeyCode.F))
            ThrowBlock();

        HookMovementCircle();
    }

    float startDistanceHook;
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
        BlockHalfWidth = BlockWidth / 2;

        Vector3 bottomScreenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0f, Camera.main.nearClipPlane));
        Vector3 di = hookTransform.position + bottomScreenPosition;
        startDistanceHook = Vector3.Distance(hookTransform.position, bottomScreenPosition);
    }

    public void OnExitMenu()
    {
        BlockCreater.DestroyAll();
        CameraMovement.Restart();
        HookRestart();
        TowerRestart();
    }

    public void BlockMiss() => CreateNewBlock();

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
        if (hookTransform.childCount >= 1) return;

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
            new Vector3(hookCenterPos.x, TopBlockPos.y + CameraOffsetMovement),
            1);

        CameraMovement.Move();

        HookIncreaseSpeed();
    }

    public void TowerAddBlock(IBlock buildingBlock)
    {
        tower.Add(buildingBlock);
        
        HeightBuilding++;

        if (HeightBuilding == 2)
        {
            tower[0].Strengthen();
            tower[1].SetBreakTorque();
        }
        else if (HeightBuilding > 9 && HeightBuilding % 5 == 0) CleanTower(5);

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
            IBlock buildingBlock = tower[i];
            buildingBlock.Deactivate();
        }

        tower = tower.Skip(5).ToList();

        tower[0].Strengthen();
        tower[1].SetBreakTorque();
    }

    #endregion
}
