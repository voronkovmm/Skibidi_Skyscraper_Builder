using DG.Tweening;
using System.Collections;
using UnityEngine;
using Zenject;

// Реализовать: вычет очков при падении блока в методе 
public class BuildingBlock : MonoBehaviour
{
    private int poolIndex;
    private bool isFalling = true;
    private BlockPool pool;
    private SpriteRenderer spriteRenderer;
    private PopupTextService popupText;
    private BuildingManager buildingManager;
    private Rigidbody2D rigidbody2d;
    private FixedJoint2D joint;
    private Tween tween;

    public class Factory : PlaceholderFactory<BuildingBlock>{}
    [Inject]
    private void Construct(PopupTextService popupText, BuildingManager buildingManager)
    {
        this.popupText = popupText;
        this.buildingManager = buildingManager;
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        joint = gameObject.AddComponent<FixedJoint2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(buildingManager.HeightBuilding == 0)
        {
            isFalling = false;
            buildingManager.AddBuildingBlock(this);
            return;
        }

        if(collision.collider.TryGetComponent(out BuildingBlock component) && isFalling)
        {
            isFalling = false;
            rigidbody2d.velocity = Vector3.zero;

            StartCoroutine(WaitingFixedJoint(collision.transform, collision.rigidbody));
            CalculateScore(collision);
        }
    }
    public void Activate(Vector3 position)
    {
        gameObject.SetActive(true);
        joint.enabled = false;
        joint.connectedBody = null;
        rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
        isFalling = true;
        transform.position = position;
    }
    public void Deactivate()
    {
        if(tween != null) tween.Kill();
        pool.Return(this, poolIndex);
        gameObject.SetActive(false);
    }
    public void Initialize(BlockPool pool, int poolIndex, Sprite sprite)
    {
        this.pool = pool;
        this.poolIndex = poolIndex;
        spriteRenderer.sprite = sprite;
        gameObject.SetActive(false);
    }
    public void SetBreakTorque() => joint.breakTorque = 150f;
    public void Strengthen() => rigidbody2d.bodyType = RigidbodyType2D.Static;
    public void SetRigidbodyDynamic() => rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
    private void CalculateScore(Collision2D collision)
    {
        float width = spriteRenderer.bounds.size.x;
        float distanceX = Mathf.Abs(transform.position.x - collision.transform.position.x);
        float normalizedDistance = Mathf.Clamp01(distanceX / width);
        int score = Mathf.RoundToInt(5f * (1f - normalizedDistance));
        popupText.Show(transform.position, score.ToString(), RatingColor.GetColor(score));
    }
    private IEnumerator WaitingFixedJoint(Transform collisionTransform, Rigidbody2D collisionRigidbody)
    {
        yield return new WaitForSeconds(3);

        float collisionBlockYPos = collisionTransform.position.y + collisionTransform.localScale.y / 2;
        if (transform.position.y > collisionBlockYPos)
        {
            tween = transform.DORotateQuaternion(collisionTransform.rotation, 0.2f)
                    .OnComplete(() =>
                    {
                        joint.enabled = true;
                        joint.connectedBody = collisionRigidbody;
                        buildingManager.AddBuildingBlock(this);
                    });
        }
    }
    
}