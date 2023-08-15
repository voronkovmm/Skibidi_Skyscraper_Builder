using DG.Tweening;
using System.Collections;
using UnityEngine;
using Zenject;

public class Block : MonoBehaviour, IBlock
{
    [Inject] private BuildingManager buildingManager;
    
    private int poolIndex;
    private bool isCollisionDisable;
    private Tween tween;
    private Rigidbody2D rigidbody2d;
    private BlockCreater pool;
    private FixedJoint2D joint;
    private SpriteRenderer spriteRenderer;

    public GameObject GameObject { get => gameObject; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        joint = gameObject.AddComponent<FixedJoint2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCollisionDisable) return;

        if(collision.collider.TryGetComponent(out Block block))
        {
            isCollisionDisable = true;
            rigidbody2d.velocity *= 0.5f;
            CollisionWithBuilding(collision.transform, collision.rigidbody);
            //CalculateScore(collision);
        }
        else if(buildingManager.HeightBuilding == 0)
        {
            buildingManager.TowerAddBlock(this);
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
        isCollisionDisable = false;

        spriteRenderer.flipX = Random.Range(0, 2) == 0 ? true : false;
    }

    public void Deactivate()
    {
        if (tween != null) tween.Kill();
        pool.Return(this, poolIndex);
        gameObject.SetActive(false);
    }

    public void Fall() => rigidbody2d.bodyType = RigidbodyType2D.Dynamic;

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

    private void CollisionWithBuilding(Transform collisionTransform, Rigidbody2D collisionRigidbody)
    {
        float collisionBlockYPos = collisionTransform.position.y + collisionTransform.localScale.y / 2;
        //Debug.DrawLine(collisionTransform.position, collisionTransform.position + Vector3.up * collisionTransform.localScale.y / 2, Color.red, 1000);

        // если позиция нашего блока по (Y - половина ширины) > чем (TowerBlock + половина ширины)

        if (transform.position.y > collisionBlockYPos)
        {
            tween = transform.DORotateQuaternion(collisionTransform.rotation, 0.2f)
                    .OnComplete(() =>
                    {
                        joint.enabled = true;
                        joint.connectedBody = collisionRigidbody;
                        
                        buildingManager.TowerAddBlock(this);
                    });
        }
    }

    public Transform GetTransform() => transform;
}
