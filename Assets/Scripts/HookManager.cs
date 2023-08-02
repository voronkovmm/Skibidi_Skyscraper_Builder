using DG.Tweening;
using UnityEngine;
using Zenject;

public class HookManager : MonoBehaviour
{
    [SerializeField] private float radiusY = 0.25f;
    [SerializeField] private float radiusX = 1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float intervalIncreaseSpeed = 0.05f;
    private float angle;
    private BuildingBlock currentBlock;
    [SerializeField] private float offsetSpawnPosition = 1;
    private bool isReadyThrow;

    [Header("Shake Block Animation")]
    [SerializeField] private Vector3 strengthShake;
    [SerializeField] private float durationShake = 0.2f;
    [SerializeField] private int vibratoShake = 1;

    private Tweener tweener;
    private Vector3 centerPosition;
    private Vector3 startPos;
    private BuildingManager buildingManager;
    private GameData gameData;
    private BuildingBlockFactory blockFactory;

    [Inject]
    private void Construct(BuildingManager buildingManager, GameData gameData, BuildingBlockFactory blockFactory)
    {
        this.buildingManager = buildingManager;
        this.gameData = gameData;
        this.blockFactory = blockFactory;
    }

    private void Start()
    {
        centerPosition = startPos = transform.position;
        CreateNewBlock();
    }

    private void CreateNewBlock()
    {
        BuildingBlock buildingBlock = blockFactory.NewBlock(transform.position + Vector3.down * offsetSpawnPosition);
        Transform blockTransform = buildingBlock.transform;
        currentBlock = buildingBlock;
        blockTransform.SetParent(transform);
        Vector3 currentScale = blockTransform.localScale;
        blockTransform.localScale = Vector3.one * 0.1f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blockTransform.DOScale(currentScale, 0.15f))
            .Append(blockTransform.DOPunchScale(strengthShake, durationShake, vibratoShake, 0))
            .OnComplete(() => isReadyThrow = true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) ThrowBlock();
        
        CircleMovement();
    }

    private void OnEnable()
    {
        buildingManager.OnNewBuildingBlock += OnNewBuildingBlock;
    }

    private void OnDisable()
    {
        buildingManager.OnNewBuildingBlock -= OnNewBuildingBlock;
    }

    private void CircleMovement()
    {
        angle += speed * Time.deltaTime;
        float x = centerPosition.x + radiusX * Mathf.Cos(angle);
        float y = centerPosition.y + radiusY * Mathf.Sin(angle);

        transform.position = new Vector2(x, y);
    }

    private void OnNewBuildingBlock() => Move();

    private void Move()
    {
        float height = gameData.HeightBuildingBlock;
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
    }

    public void ThrowBlock()
    {
        if(isReadyThrow && currentBlock != null)
        {
            currentBlock.SetRigidbodyDynamic();
            transform.DetachChildren();
            currentBlock = null;
            isReadyThrow = false;
            Invoke("CreateNewBlock", 0.5f);
        }
    }

    private void IncreaseSpeed() => speed += intervalIncreaseSpeed;
}