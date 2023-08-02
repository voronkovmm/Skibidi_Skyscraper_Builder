using UnityEngine;

public class RatingColor
{
    public static readonly Color FIVE = new Color(0, 100, 0);
    public static readonly Color FOUR = new Color(50, 205, 50);
    public static readonly Color THREE = new Color(255, 255, 0);
    public static readonly Color TWO = new Color(255, 165, 0);
    public static readonly Color ONE = new Color(139, 0, 0);

    public static Color GetColor(float rating)
    {
        if (rating >= 5) return FIVE;
        else if (rating >= 4 && rating < 5) return FOUR;
        else if (rating >= 3 && rating < 4) return THREE;
        else if (rating >= 2 && rating < 3) return TWO;
        else if (rating >= 1 && rating < 2) return ONE;
        else 
            return Color.white;
    }

    private RatingColor() {}
}