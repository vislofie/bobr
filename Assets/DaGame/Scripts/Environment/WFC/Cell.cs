using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Cell
{
    public Dictionary<SpawnSample, float> SamplesWithAngles { get; private set; } = new Dictionary<SpawnSample, float>();

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public Cell PositiveXNeighbour { get; private set; }
    public Cell NegativeXNeighbour { get; private set; }
    public Cell PositiveYNeighbour { get; private set; }
    public Cell NegativeYNeighbour { get; private set; }
    public Cell PositiveZNeighbour { get; private set; }
    public Cell NegativeZNeighbour { get; private set; }

    public bool Collapsed { get; private set; } = false;
    public bool FilledByNeighbour { get; private set; } = false;

    public Cell(SpawnSample[] samples, int x, int y, int z)
    {
        for (int i = 0; i < samples.Length; i++)
        {
            SamplesWithAngles.Add(samples[i], 0);
        }
        X = x;
        Y = y;
        Z = z;
    }

    public void CollapseCell()
    {
        if (SamplesWithAngles.Keys.Count > 0 && !Collapsed)
        {
            int randomIndex = UnityEngine.Random.Range(0, SamplesWithAngles.Keys.Count);
            SpawnSample chosenSample = SamplesWithAngles.Keys.ToArray()[randomIndex];
            float chosenAngle = SamplesWithAngles.Values.ToArray()[randomIndex];
            SamplesWithAngles = new Dictionary<SpawnSample, float>
            {
                { chosenSample, chosenAngle }
            };
            Collapsed = true;
        }
    }

    public void FillNeighbours(Cell positiveX, Cell negativeX, Cell positiveY, Cell negativeY, Cell positiveZ, Cell negativeZ)
    {
        PositiveXNeighbour = positiveX;
        NegativeXNeighbour = negativeX;
        PositiveYNeighbour = positiveY;
        NegativeYNeighbour = negativeY;
        PositiveZNeighbour = positiveZ;
        NegativeZNeighbour = negativeZ;
    }

    public void FillSamples(SpawnSample[] samples, float[] angles)
    {
        if (FilledByNeighbour)
        {
            for (int i = SamplesWithAngles.Count - 1; i >= 0; i--)
            {
                if (!samples.Contains(SamplesWithAngles.Keys.ToArray()[i]))
                {
                    SamplesWithAngles.Remove(SamplesWithAngles.Keys.ToArray()[i]);
                }
            }
            return;
        }

        if (!Collapsed)
        {
            if (samples.Length != angles.Length)
            {
                return;
            }

            SamplesWithAngles = new Dictionary<SpawnSample, float>();

            for (int i = 0; i < samples.Length; i++)
            {
                SamplesWithAngles.Add(samples[i], angles[i]);
            }

            FilledByNeighbour = true;
        }

    }

}
