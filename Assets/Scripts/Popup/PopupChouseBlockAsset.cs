using System;
using UnityEngine;
using Zenject;

public class PopupChouseBlockAsset : MonoBehaviour
{
    [Inject] private ViewMenu viewMenu;
    [Inject] private ViewGame viewGame;
    [Inject] private AccountManager dataAccount;

    [SerializeField] private GameObject container;
    [SerializeField] private GameObject containerUnlockAsset;

    private EnumSkinAsset selectedSkin;

    public void Open()
    {
        container.SetActive(true);
    }

    public void Close()
    {
        container.SetActive(false);
    }

    public void BtnChouseAsset(int number)
    {
        selectedSkin = Enum.Parse<EnumSkinAsset>(number.ToString());

        if (!dataAccount.DataSkinAsset.IsUnlock(selectedSkin))
        {
            containerUnlockAsset.SetActive(true);
            return;
        }

        dataAccount.DataSkinAsset.SetCurrent(selectedSkin);
    }

    public void BtnUnlockAsset()
    {
        // реклама

#if UNITY_EDITOR
        dataAccount.DataSkinAsset.Unlock(selectedSkin);
        dataAccount.DataSkinAsset.SetCurrent(selectedSkin);
        Debug.Log($"<color=red>[DataSkinAsset]</color>: Скин {selectedSkin} разблокирован");
#endif

        containerUnlockAsset.SetActive(false);
    }

    public void BtnStartGame()
    {
        viewGame.Open();
        viewMenu.Close();
        Close();
    }
}
