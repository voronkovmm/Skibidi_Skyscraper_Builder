using TMPro;
using UnityEngine;

public class WidgetFixation : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpCounter;
    
    public ViewGame ViewGame { get; private set; }

    private int count = 3;

    public void Initialize(ViewGame viewGame)
    {
        ViewGame = viewGame;
    }

    public void OnClick() => Fixation();

    public void Fixation()
    {
        if (count <= 0) return;

        tmpCounter.text = $"{--count}";

        ViewGame.BuildingManager.AbilityFixation();
    }
}
