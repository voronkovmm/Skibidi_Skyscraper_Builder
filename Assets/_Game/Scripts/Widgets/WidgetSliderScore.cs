using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WidgetSliderScore : MonoBehaviour
{
    [Inject] private BuildingManager buildingManager;

    [SerializeField] private Slider sliderScore;
    [SerializeField] private Gradient sliderScoreGradient;
    [SerializeField] private Image sliderScoreBackgroundImage;

    public bool IsNegativeFactor;
    public bool IsPositiveFactor;
    private int negativeFactorCounter;
    private int positiveFactorCounter;
    private float maxFactorCounter = 2;
    private float factorValue = 3.5f;
    private float sumScores;
    private float currentScore;
    private Tween tweenScore;

    public void Initiailize()
    {
        sumScores = 0;
        ResetSliderScore();
        FactorWasBeUsed();
    }

    public void FillScore(float score)
    {
        sumScores += score;
        currentScore = sumScores / buildingManager.TotalHeightBuilding;
        tweenScore.Kill();
        tweenScore = DOTween.To(() => sliderScore.value, x => sliderScore.value = x, currentScore, 1f);
        sliderScoreBackgroundImage.color = sliderScoreGradient.Evaluate((currentScore - sliderScore.minValue) / (sliderScore.maxValue - sliderScore.minValue));
        CalculateFactors();
    }

    public void DecreaseScore(float value)
    {
        currentScore -= value;
        tweenScore.Kill();
        tweenScore = DOTween.To(() => sliderScore.value, x => sliderScore.value = x, currentScore, 1f);
        sliderScoreBackgroundImage.color = sliderScoreGradient.Evaluate((currentScore - sliderScore.minValue) / (sliderScore.maxValue - sliderScore.minValue));
        CalculateFactors();
    }

    public void RecalculateScore()
    {
        sumScores = buildingManager.TotalHeightBuilding * currentScore;
    }

    public void FactorWasBeUsed()
    {
        IsPositiveFactor = false;
        IsNegativeFactor = false;
        negativeFactorCounter = 0;
        positiveFactorCounter = 0;
    }

    private void ResetSliderScore()
    {
        sliderScore.value = sumScores;
        sliderScoreBackgroundImage.color = sliderScoreGradient.Evaluate(1);
    }

    private void CalculateFactors()
    {
        if (IsPositiveFactor || IsNegativeFactor) return;

        bool isPositiveFactor = currentScore > factorValue;

        if (isPositiveFactor)
        {
            positiveFactorCounter++;
            negativeFactorCounter = 0;
        }
        else
        {
            negativeFactorCounter++;
            positiveFactorCounter = 0;
        }

        if (positiveFactorCounter >= maxFactorCounter)
        {
            IsPositiveFactor = true;
        }
        else if (negativeFactorCounter >= maxFactorCounter)
        {
            IsNegativeFactor = true;
        }
    }
}
