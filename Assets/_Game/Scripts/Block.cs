using DG.Tweening;
using System.Collections;
using UnityEngine;
using Zenject;

public class Block : MonoBehaviour, IBlock
{
    [Inject] private BuildingManager buildingManager;
    
    private int poolIndex;
    private bool isConnectedToBuilding;
    private bool isMiss;

    private Tween tween;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    private BlockCreater pool;
    private FixedJoint2D joint;
    private SpriteRenderer spriteRenderer;

    public GameObject GameObject { get => gameObject; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        joint = gameObject.AddComponent<FixedJoint2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isConnectedToBuilding 
            || (buildingManager.HeightBuilding > 0 && buildingManager.TopBlockPos.y > transform.position.y)) return;

        if (collision.collider.TryGetComponent(out Block block))
        {
            Debug.Log($"выполнился коллайдер {gameObject.name}");
            rigidbody2d.velocity *= 0.25f;
            CollisionWithBuilding(collision.transform, collision.rigidbody);
            //CalculateScore(collision);
        }
        else if (buildingManager.HeightBuilding == 0)
        {
            buildingManager.TowerAddBlock(this);
            isConnectedToBuilding = true;
            return;
        }
    }

    public void Initialize(BlockCreater pool, int poolIndex, Sprite sprite)
    {
        this.pool = pool;
        this.poolIndex = poolIndex;
        spriteRenderer.sprite = sprite;
        gameObject.SetActive(false);
    }

    public void Activate(Vector3 position)
    {
        gameObject.SetActive(true);
        joint.enabled = false;
        joint.connectedBody = null;
        rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isConnectedToBuilding = false;
        boxCollider2d.enabled = true;
        isMiss = false;

        spriteRenderer.flipX = Random.Range(0, 2) == 0 ? true : false;
    }

    public void Deactivate()
    {
        if (tween != null) tween.Kill();
        // нужно убрать сцепку
        pool.Return(this, poolIndex);
        gameObject.SetActive(false);
    }

    public IEnumerator RoutineDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);

        Deactivate();
    }

    public void Fall()
    {
        StartCoroutine(CheckPosition());
        rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
    }

    public void SetParent(Transform parent) => transform.SetParent(parent);

    public void SetBreakTorque() => joint.breakTorque = 150f;

    public void Strengthen() => rigidbody2d.bodyType = RigidbodyType2D.Static;

    private void CalculateScore(Collision2D collision)
    {
        float width = spriteRenderer.bounds.size.x;
        float distanceX = Mathf.Abs(transform.position.x - collision.transform.position.x);
        float normalizedDistance = Mathf.Clamp01(distanceX / width);
        int score = Mathf.RoundToInt(5f * (1f - normalizedDistance));
        //popupText.Show(transform.position, score.ToString(), RatingColor.GetColor(score));
    }

    // отслеживаем его - если он пролетел верхний блок - ставим его в мис
    private void CollisionWithBuilding(Transform collisionTransform, Rigidbody2D collisionRigidbody)
    {
        float distanceX = Mathf.Abs(transform.position.x - collisionTransform.transform.position.x);

        bool isMiss = distanceX > buildingManager.BlockHalfWidth;

        if (!isMiss)
        {
            joint.enabled = true;
            joint.connectedBody = collisionRigidbody;

            isConnectedToBuilding = true;
            buildingManager.TowerAddBlock(this);
        }
        else
            MissBlock();
    }

    private IEnumerator CheckPosition()
    {
        if (buildingManager.HeightBuilding == 0) yield break;

        float timer = 3;
        while((timer -= Time.deltaTime) > 0)
        {
            if (isConnectedToBuilding || isMiss) yield break;

            if(transform.position.y < buildingManager.TopBlockPos.y) break;

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        MissBlock();
    }

    private void MissBlock()
    {
        if (isMiss) return;

        isMiss = true;
        boxCollider2d.enabled = false;
        Vector3 jumpDirection = transform.position.x > buildingManager.TopBlockPos.x ? Vector3.right : Vector3.left;

        Sequence sequence = DOTween.Sequence();
        float duration = 1f;
        sequence
            .Append(transform.DOJump(transform.position + jumpDirection * 2, 1, 1, duration))
            .Insert(0, transform.DOMoveY(transform.position.y - 10, duration).SetEase(Ease.InCubic))
            .Insert(0, transform.DORotate(Vector3.forward * -jumpDirection.x * 180, duration).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                buildingManager.BlockMiss();
                Deactivate();
            });
    }

    public Transform GetTransform() => transform;
}
