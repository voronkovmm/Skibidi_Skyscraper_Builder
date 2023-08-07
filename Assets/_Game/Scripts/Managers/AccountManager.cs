using UnityEngine;
using Zenject;

// сделать получение текущего ассета от сюда а не из DataManager
public class AccountManager : MonoBehaviour
{
    public DataSkinAsset DataSkinAsset { get; private set; } = new();
}
