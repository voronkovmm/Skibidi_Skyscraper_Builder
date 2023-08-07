using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class PopupText : MonoBehaviour
{
    [SerializeField] private float startFontSize = 2;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float lifeTime = 0.3f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Color textColor;
    private TMP_Text tmp;
    private PopupTextPool pool;
    Sequence sequence;

    private void Awake()
    {
        tmp = GetComponentInChildren<TMP_Text>();
        tmp.fontSize = startFontSize;
        tmp.color = textColor;

        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void Initialize(PopupTextPool pool) => this.pool = pool;

    public void Activate(Vector3 startPos, string text, Color color)
    {
        Vector3 randomOffset = Random.Range(0, 2) == 0 ? -offset : offset;
        transform.position = startPos + randomOffset;

        tmp.text = text;
        tmp.color = color;
        gameObject.SetActive(true);

        sequence.Kill();
        sequence = DOTween.Sequence()
           .Append(transform.DOMove(moveDirection, lifeTime).SetRelative(true))
           .Insert(0, transform.DOScale(0.01f, lifeTime).SetDelay(lifeTime * 0.9f))
           .OnComplete(() =>
           {
               transform.localScale = Vector3.one;
               Deactivate();
           });
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        pool.Return(this);
    }
}
