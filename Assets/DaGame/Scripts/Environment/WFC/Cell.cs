using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[Serializable]
public class Cell
{
    public List<SpawnSample> Samples { get; private set; } = new List<SpawnSample>();

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public bool Collapsed { get; private set; } = false;

    public Cell(SpawnSample[] samples, int x, int y, int z)
    {
        for (int i = 0; i < samples.Length; i++)
        {
            Samples.Add(samples[i]);
        }
        X = x;
        Y = y;
        Z = z;
    }

    public void CollapseCell()
    {
        if (Samples.Count > 0 && !Collapsed)
        {
            SpawnSample chosenSample = null;

            float sumWeight = Samples.Select(sample => sample.RandomChance).Sum();
            float emptyOnInitSumWeight = 0.0f;

            float randomWeight = UnityEngine.Random.Range(0, sumWeight);

            if (Samples.Count > 1)
            {
                for (int i = 0; i <= Samples.Count; i++)
                {
                    if (emptyOnInitSumWeight > randomWeight)
                    {
                        chosenSample = Samples[i - 1];
                        break;
                    }
                    emptyOnInitSumWeight += Samples[i].RandomChance;
                }
            }
            

            Samples = new List<SpawnSample> { chosenSample };
            Collapsed = true;
        }
    }

    public void CompareSamples()
    {

    }
}
