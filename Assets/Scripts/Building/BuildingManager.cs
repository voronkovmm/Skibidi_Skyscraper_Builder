using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private float cooldownNewBlock = 0.5f;
    [SerializeField] private HookManager hook;
    
    public event Action OnNewBuildingBlock;
    public int HeightBuilding { get; private set; }
    public Queue<BuildingBlock> blocks = new();

    public void AddBuildingBlock(BuildingBlock buildingBlock)
    {
        HeightBuilding++;
        Transform child = buildingBlock.transform;

        blocks.Enqueue(buildingBlock);
        OnNewBuildingBlock?.Invoke();

        if (HeightBuilding == 2)
        {
            blocks.Peek().Strengthen();
            blocks.ElementAt(1).SetBreakTorque();
        }
        else if (HeightBuilding % 10 == 0) CleanTower(5);
    }

    private void CleanTower(int count)
    {
        for (int i = 0; i < count; i++)
        {
            BuildingBlock buildingBlock = blocks.Dequeue();
            buildingBlock.Deactivate();
        }

        blocks.Peek().Strengthen();
            blocks.ElementAt(1).SetBreakTorque();
    }
}