public class DataSkinAsset
{
    public EnumSkinAsset CurrentAsset { get; private set; }
    public bool IsSkibidiUnlocked { get; private set; }
    public bool IsBarbieUnlocked { get; private set; }

    public void SetCurrent(EnumSkinAsset skinType) => CurrentAsset = skinType;

    public void Unlock(EnumSkinAsset skinType)
    {
        switch (skinType)
        {
            case EnumSkinAsset.BARBIE: IsBarbieUnlocked = true;
                break;
            case EnumSkinAsset.SKIBIDI: IsSkibidiUnlocked = true;
                break;
        }
    }

    public bool IsUnlock(EnumSkinAsset skinType)
    {
        switch (skinType)
        {
            case EnumSkinAsset.BARBIE:
                return IsBarbieUnlocked;
            case EnumSkinAsset.SKIBIDI:
                return IsSkibidiUnlocked;
        }

        return false;
    }
}