using System;
using UnityEngine;

[Serializable]
public class Cell : MonoBehaviour
{
    public SpawnSample[] Samples { get; private set; }

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public Cell(SpawnSample[] samples, int x, int y, int z)
    {
        Samples = samples;
        X = x;
        Y = y;
        Z = z;
    }

    public void CollapseCell()
    {
        SpawnSample chosenSample = Samples[UnityEngine.Random.Range(0, Samples.Length - 1)];
        Samples = new SpawnSample[] { chosenSample };
    }
}
