using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class WFCSpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnSample[] _spawnSamples;
    [SerializeField]
    private float _cellScale = 1.0f;

    [SerializeField]
    private int _gridWidth = 8;
    [SerializeField]
    private int _gridLength = 8;
    [SerializeField]
    private int _gridHeight = 3;

    [SerializeField]
    private float _visualizerPauseValue = 2.0f;

    private Vector3 _currentCubePos = Vector3.zero;

    private List<Cell> _cells;

    private bool _startedCollapse = false;

    private void OnValidate()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        if (!_startedCollapse)
        {
            _cells = new List<Cell>();

            for (int i = 0; i < _gridWidth * _gridLength * _gridHeight; i++)
            {
                int currentXCoord = i % _gridWidth;
                int currentYCoord = i / (_gridWidth * _gridLength);
                int currentZCoord = i / _gridWidth - currentYCoord * _gridLength;

                _cells.Add(new Cell(_spawnSamples, currentXCoord, currentYCoord, currentZCoord));
            }

            for (int i = 0; i < _cells.Count; i++)
            {
                Cell currentCell = _cells[i];

                currentCell.FillNeighbours(TakeCellByPosition(currentCell.X + 1, currentCell.Y, currentCell.Z),
                                           TakeCellByPosition(currentCell.X - 1, currentCell.Y, currentCell.Z),
                                           TakeCellByPosition(currentCell.X, currentCell.Y + 1, currentCell.Z),
                                           TakeCellByPosition(currentCell.X, currentCell.Y - 1, currentCell.Z),
                                           TakeCellByPosition(currentCell.X, currentCell.Y, currentCell.Z + 1),
                                           TakeCellByPosition(currentCell.X, currentCell.Y, currentCell.Z - 1));
            }
        }
    }

    public void CollapseGrid()
    {
        StopCollapsing();
        _startedCollapse = true;
        StartCoroutine(CollapseVisualizer(_visualizerPauseValue));
    }

    public void StopCollapsing()
    {
        _startedCollapse = false;
        StopAllCoroutines();
    }

    private IEnumerator CollapseVisualizer(float timeToWait)
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            Cell currentCell = TakeCellWithSmallestEntropy();
            if (currentCell == null) break;

            currentCell.CollapseCell();

            _currentCubePos = new Vector3(currentCell.X, currentCell.Y, currentCell.Z) * _cellScale * 2;

            yield return new WaitForSeconds(_visualizerPauseValue);

            if (currentCell.SamplesWithAngles.Keys.Count > 0)
            {
                SpawnSample currentCellSpawnSample = currentCell.SamplesWithAngles.Keys.First();

                if (currentCellSpawnSample != null)
                {
                    // TODO: fill samples of neighbours considering local rotation of current cell
                    currentCell.PositiveXNeighbour?.FillSamples(currentCellSpawnSample.XFaceAllowed.Select(ao => ao.Sample).ToArray(),
                                                           currentCellSpawnSample.XFaceAllowed.Select(ao => ao.AtAnAngle).ToArray());
                    yield return new WaitForSeconds(_visualizerPauseValue / 2);

                    currentCell.NegativeXNeighbour?.FillSamples(currentCellSpawnSample.NegativeXFaceAllowed.Select(ao => ao.Sample).ToArray(),
                                                               currentCellSpawnSample.NegativeXFaceAllowed.Select(ao => ao.AtAnAngle).ToArray());
                    yield return new WaitForSeconds(_visualizerPauseValue / 2);
                    currentCell.PositiveYNeighbour?.FillSamples(currentCellSpawnSample.YFaceAllowed.Select(ao => ao.Sample).ToArray(),
                                                                currentCellSpawnSample.YFaceAllowed.Select(ao => ao.AtAnAngle).ToArray());
                    yield return new WaitForSeconds(_visualizerPauseValue / 2);
                    currentCell.NegativeYNeighbour?.FillSamples(currentCellSpawnSample.NegativeYFaceAllowed.Select(ao => ao.Sample).ToArray(),
                                                                currentCellSpawnSample.NegativeYFaceAllowed.Select(ao => ao.AtAnAngle).ToArray());
                    yield return new WaitForSeconds(_visualizerPauseValue / 2);
                    currentCell.PositiveZNeighbour?.FillSamples(currentCellSpawnSample.ZFaceAllowed.Select(ao => ao.Sample).ToArray(),
                                                               currentCellSpawnSample.ZFaceAllowed.Select(ao => ao.AtAnAngle).ToArray());
                    yield return new WaitForSeconds(_visualizerPauseValue / 2);
                    currentCell.NegativeZNeighbour?.FillSamples(currentCellSpawnSample.NegativeZFaceAllowed.Select(ao => ao.Sample).ToArray(),
                                                               currentCellSpawnSample.NegativeZFaceAllowed.Select(ao => ao.AtAnAngle).ToArray());
                    yield return new WaitForSeconds(_visualizerPauseValue / 2);

                    Debug.Log(currentCell.Collapsed);
                    Instantiate(currentCellSpawnSample.SpawnPrefab, (new Vector3(currentCell.X, currentCell.Y, currentCell.Z) - Vector3.up / 2) * _cellScale * 2, Quaternion.Euler(0, currentCell.SamplesWithAngles[currentCellSpawnSample], 0));
                }
            }

            yield return new WaitForSeconds(_visualizerPauseValue * 1.5f);
        }
        _startedCollapse = false;
    }

    private Cell TakeCellWithSmallestEntropy()
    {
        int minimumEntropy = int.MaxValue;
        int cellIndex = -1;

        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
        Cell[] shuffledArray = _cells.OrderBy(x => rnd.Next()).ToArray();

        for (int i = 0; i < shuffledArray.Length; i++)
        {
            if (shuffledArray[i].SamplesWithAngles.Keys.Count < minimumEntropy  && shuffledArray[i].SamplesWithAngles.Keys.Count > 0 && !shuffledArray[i].Collapsed)
            {
                minimumEntropy = shuffledArray[i].SamplesWithAngles.Count;
                cellIndex = FromPositionToIndex(shuffledArray[i].X, shuffledArray[i].Y, shuffledArray[i].Z);
            }
        }

        if (cellIndex != -1)
        {
            Cell returnCell = _cells[cellIndex];
            return returnCell;
        }

        return null;
    }

    private int FromPositionToIndex(int x, int y, int z)
    {
        return x + z * _gridWidth + y * (_gridWidth * _gridLength);
    }

    private Cell TakeCellByPosition(int x, int y, int z)
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            if (_cells[i].X == x && _cells[i].Y == y && _cells[i].Z == z)
            {
                return _cells[i];
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Color transparentYellow = Color.yellow;
        transparentYellow.a = 0.1f;
        for (int i = 0; i < _cells.Count; i++)
        {
            Gizmos.color = Color.yellow;

            if (_cells[i].Collapsed)
                Gizmos.color = Color.red;

            Vector3 cellPos = new Vector3(_cells[i].X, _cells[i].Y, _cells[i].Z) * _cellScale * 2;
            Gizmos.DrawWireCube(cellPos, Vector3.one * _cellScale * 2);

            Gizmos.color = transparentYellow;
            Gizmos.DrawCube(_currentCubePos, Vector3.one * _cellScale * 2);

            Gizmos.color = Color.blue;

            if (_cells[i].SamplesWithAngles.ContainsKey(_spawnSamples[0]))
            {
                Gizmos.DrawMesh(_spawnSamples[0].SpawnPrefab.GetComponentInChildren<MeshFilter>().sharedMesh, 
                                    cellPos - new Vector3(0.5f, 0.0f, 0.0f),
                                    Quaternion.Euler(-90, _cells[i].SamplesWithAngles[_spawnSamples[0]], 0),
                                    Vector3.one * 25.0f * _cellScale);
            }
            if (_cells[i].SamplesWithAngles.ContainsKey(_spawnSamples[1]))
            {
                Gizmos.DrawMesh(_spawnSamples[1].SpawnPrefab.GetComponentInChildren<MeshFilter>().sharedMesh, 
                                    cellPos + new Vector3(0.5f, 0.0f, 0.0f), 
                                    Quaternion.Euler(-90, _cells[i].SamplesWithAngles[_spawnSamples[1]], 0),
                                    Vector3.one * 25.0f * _cellScale);
            }

            if (_cells[i].SamplesWithAngles.ContainsKey(_spawnSamples[2]))
            {
                Gizmos.DrawMesh(_spawnSamples[2].SpawnPrefab.GetComponentInChildren<MeshFilter>().sharedMesh,
                                    cellPos + new Vector3(0.0f, 0.75f, 0.0f),
                                    Quaternion.Euler(0, _cells[i].SamplesWithAngles[_spawnSamples[2]], 0),
                                    Vector3.one / 2 * _cellScale);
            }
        }
    }
}
