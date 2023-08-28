using DG.Tweening;
using UnityEngine;
using Zenject;

public class CitizenBarbie : Citizen
{
    [Inject] private BuildingManager BuildingManager;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    public override void Activate()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        gameObject.SetActive(true);

        Vector3 screenBlockPos = cameraMain.WorldToScreenPoint(BuildingManager.TopBlockPos);
        Vector3 spawnPos = cameraMain.ScreenToWorldPoint(new Vector3(-50, Random.Range(screenBlockPos.y - 100, screenBlockPos.y + 100)));
        transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);

        Sequence sequ = DOTween.Sequence();
        sequ.Append(transform.DOMoveY(BuildingManager.TopBlockPos.y, Random.Range(1f, 3f)).SetEase(Ease.InOutCubic))
            .Insert(0, transform.DOMoveX(BuildingManager.TopBlockPos.x, Random.Range(1f, 3f)).SetEase(Ease.OutQuad))
            .Insert(0, transform.DOScale(0.65f, 1.5f))
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                Deactivate();
            });
    }
}