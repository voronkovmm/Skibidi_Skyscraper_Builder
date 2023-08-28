using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BuildingManager : MonoCash
{
    [Inject] private AccountManager AccountManager;
    [Inject] private BlockCreator BlockCreater;
    [Inject] private CitizenCreator CitizenCreator;
    [Inject] private CameraMovement CameraMovement;
    [Inject] private ViewGame ViewGame;
    [Inject] private ViewMenu ViewMenu;
    [Inject] private WidgetSliderScore WidgetSliderScore;

    [Header("Main")]
    [SerializeField] public int SkipBlocksForMovement = 1;
    [SerializeField] public float CameraOffsetMovement = 9;
    [SerializeField] private Transform prefabRoof;
    private bool isEndGame;
    private bool IsFactorWorking;

    [Header("Hook")]
    [SerializeField] private float hookRadiusY = 0.25f;
    [SerializeField] private float hookRadiusX = 1f;
    [SerializeField] private float hookMovementSpeed = 2f;
    [SerializeField] private float hookIntervalIncreaseSpeed = 0.05f;
    [SerializeField] private Transform hookTransform;
    private float hookCurrentMovementSpeed;
    private float hookAngleMovement;
    private bool isHookControlLock;
    private bool isHookCircleMovementLock;
    private Vector3 hookCenterPos;
    private Vector3 hookStartPos;
    private Tweener hookTweenerMovement;

    [Header("Block")]
    [SerializeField] public float AttachmentDelay = 2;
    [SerializeField] private float offsetSpawnY = 1;
    [SerializeField] private Transform ParentBlock;
    public float BlockHeight { get; private set; }
    public float BlockWidth { get; private set; }
    public float BlockHalfWidth { get; private set; }
    private bool isNewBlockReady;
    private IBlock blockCurrent;
    private int counterMissBlocks;

    [Header("Shake Block Animation")]
    [SerializeField] private int blockVibratoShake = 1;
    [SerializeField] private float blockDurationShake = 0.2f;
    [SerializeField] private Vector3 blockStrengthShake;

    

    public Vector3 TopBlockPos{ get => tower[^1].GetTransform().position; }


    public int TotalHeightBuilding { get; private set; }
    public int HeightBuilding { get => transform.childCount; }

    private List<IBlock> tower = new();

    public override void OnTick()
    {
        if (!ViewGame.IsGame) return;

        if (Input.GetKeyDown(KeyCode.F) && !isHookControlLock)
            ThrowBlock();

        HookMovementCircle();
    }

    public void Initialize()
    {
        isEndGame = false;
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
        float startDistanceHook = Vector3.Distance(hookTransform.position, bottomScreenPosition);

        counterMissBlocks = 0;
        isHookControlLock = false;
        isHookCircleMovementLock = false;
        IsFactorWorking = false;
    }

    public void OnExitMenu()
    {
        BlockCreater.DestroyAll();
        CameraMovement.Restart();
        HookRestart();
        TowerRestart();
    }

    public void BlockMiss()
    {
        if (isEndGame) return;

        WidgetSliderScore.DecreaseScore(1);

        if(++counterMissBlocks >= 3)
        {
            isEndGame = true;
            StartCoroutine(EndGame());
            return;
        };

        if (isHookControlLock) return;

        CreateNewBlock();
    }

    public void ThrowBlock()
    {
        if (!isNewBlockReady || isEndGame) return;

        if (!IsFactorWorking)
        {
            if (WidgetSliderScore.IsPositiveFactor)
            {
                WidgetSliderScore.FactorWasBeUsed();
            }
            else if (WidgetSliderScore.IsNegativeFactor)
            {
                StartCoroutine(NegativeFactorManyBlocks());
            }
        }

        hookTransform.DetachChildren();

        blockCurrent.Fall();

        blockCurrent = null;
        isNewBlockReady = false;
    }

    private void CreateNewBlock()
    {
        if (hookTransform.childCount >= 1 || isEndGame) return;

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
        if (isHookCircleMovementLock) return;
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

    private void MovemetHookAndCamera()
    {
        if (TotalHeightBuilding < SkipBlocksForMovement
            || isHookCircleMovementLock) return;

        hookTweenerMovement.Kill();

        hookTweenerMovement = DOTween.To(
            () => hookCenterPos,
            x => hookCenterPos = x,
            new Vector3(hookCenterPos.x, TopBlockPos.y + CameraOffsetMovement),
            1);

        CameraMovement.Move();

        HookIncreaseSpeed();
    }

    public void AddBlock(IBlock buildingBlock)
    {
        if (isEndGame) return;

        tower.Add(buildingBlock);
        
        TotalHeightBuilding++;
        
        CalculateScore();
        

        if (TotalHeightBuilding > 9 && TotalHeightBuilding % 5 == 0) 
            CleanTower(5);
        else if (TotalHeightBuilding == 2)
        {
            tower[0].Strengthen();
        }
        else if (TotalHeightBuilding == 3)
        {
            tower[1].SetBreakTorque();
        }

        MovemetHookAndCamera();

        if (isHookControlLock) return;

        CreateNewBlock();
    }

    public void CalculateScore()
    {
        if (TotalHeightBuilding < 2)
        {
            WidgetSliderScore.FillScore(5);
            for (int i = 0; i < 5; i++) CitizenCreator.Get();
            return;
        }
        float distanceX = Mathf.Abs(tower[^1].GetTransform().position.x - tower[^2].GetTransform().position.x);
        float normalizedDistance = Mathf.Clamp01(distanceX / BlockHalfWidth);
        float score = 5f * (1f - normalizedDistance);
        WidgetSliderScore.FillScore(score);

        for (int i = 0; i < Mathf.FloorToInt(score); i++)
        {
            CitizenCreator.Get(); 
        }
    }

    private void TowerRestart()
    {
        TotalHeightBuilding = 0;
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

        WidgetSliderScore.RecalculateScore();
    }

    #endregion

    private IEnumerator EndGame()
    {
        isHookControlLock = true;
        isHookCircleMovementLock = true;

        if(hookTransform.childCount > 0)
        {
            Transform transform1 = hookTransform.GetChild(0);
            transform1.GetComponent<IBlock>().Deactivate();
            hookTransform.GetChild(0).DetachChildren();
        }

        hookTransform.DOMove(new Vector3(tower[^1].GetTransform().position.x, hookTransform.position.y), 0.5f);
        
        yield return new WaitForSeconds(1);


        Transform roof = Instantiate(prefabRoof, hookTransform.position + Vector3.down * offsetSpawnY, Quaternion.identity, hookTransform);
        Vector3 initialScale = roof.localScale;
        roof.localScale = Vector3.one * 0.1f;
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(roof.DOScale(initialScale, 0.15f))
            .Append(roof.DOPunchScale(blockStrengthShake, blockDurationShake, blockVibratoShake, 0));

        yield return new WaitForSeconds(1);

        hookTransform.DetachChildren();
        Rigidbody2D rigidbody2D1 = roof.GetComponent<Rigidbody2D>();
        rigidbody2D1.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(2);

        Destroy(roof.gameObject);
        ViewGame.Close();
        ViewMenu.Open();
    }

    private IEnumerator NegativeFactorManyBlocks()
    {
        float delay = 0.5f;
        isHookControlLock = true;
        isHookCircleMovementLock = true;
        IsFactorWorking = true;

        int maxBlocks = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < maxBlocks; i++)
        {
            if (isEndGame) yield break;

            yield return new WaitForSeconds(delay);
            CreateNewBlock();

            if (!isNewBlockReady)
            {
                while (!isNewBlockReady)
                {

                    yield return null;
                }
            }

            ThrowBlock();
        }

        yield return new WaitForSeconds(delay);

        if (isEndGame) yield break;

        CreateNewBlock();
        isHookControlLock = false;
        isHookCircleMovementLock = false;
        IsFactorWorking = false;
        MovemetHookAndCamera();
        WidgetSliderScore.FactorWasBeUsed();
    }

    #region Ability

    public void AbilityFixation()
    {
        float xPos = tower[0].GetTransform().position.x;

        tower.ForEach(x => {
            x.Fixation();
            x.GetTransform().DOMove(new Vector3(xPos, x.GetTransform().position.y), 0.5f);
        });
    }

    #endregion
}
