using DG.Tweening;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Zenject;

public class ViewGame : MonoBehaviour
{
    [Inject] private PopupPause popupPause;
    [Inject] private BuildingManager buildingManager;

    [SerializeField] private GameObject container;
    [SerializeField] private Slider sliderScore;
    [SerializeField] private Gradient sliderScoreGradient;
    [SerializeField] private Image sliderScoreBackgroundImage;

    private float sumScores;
    private float numberScores;
    private Tween tweenScore;

    [HideInInspector] public bool IsGame;

    public void BtnPause() => popupPause.Open();

    public void Open()
    {
        IsGame = true;
        ResetSliderScore();
        buildingManager.Initialize();
        container.SetActive(true);
    }

    public void Close()
    {
        IsGame = false;
        buildingManager.OnExitMenu();
        container.SetActive(false);
    }

    public void FillScore(float score)
    {
        Debug.Log($"score {score}, sum {sumScores}");
        numberScores++;
        sumScores += score;
        float newScore = sumScores / numberScores;
        tweenScore.Kill();
        DOTween.To(() => sliderScore.value, x => sliderScore.value = x, newScore, 1f);
        sliderScoreBackgroundImage.color = sliderScoreGradient.Evaluate((newScore - sliderScore.minValue) / (sliderScore.maxValue - sliderScore.minValue));
    }

    private void ResetSliderScore()
    {
        sliderScore.value = sumScores = sliderScore.maxValue;
        numberScores = 1;
        sliderScoreBackgroundImage.color = sliderScoreGradient.Evaluate(1);
    }
}