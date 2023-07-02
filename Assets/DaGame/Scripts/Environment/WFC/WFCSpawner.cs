using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Xml;

[ExecuteInEditMode]
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

    private Vector3 _currentCubePos = Vector3.zero;
    private Vector3 _currentNeighbourPos = Vector3.zero;

    private List<Cell> _cells;
    private Stack<Cell> _stackedCells = new Stack<Cell>();

    private bool _startedCollapse = false;
    

    private void Update()
    {
        InitializeGrid();
    }

    /// <summary>
    /// fills grid and its cells coordinates
    /// </summary>
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
        }
    }

    /// <summary>
    /// Starts the process of collapsing the whole grid
    /// </summary>
    public void CollapseGrid()
    {
        StopCollapsing();
        _startedCollapse = true;
        Collapse();
    }

    /// <summary>
    /// Forcefully stops collapsion
    /// </summary>
    public void StopCollapsing()
    {
        _startedCollapse = false;
        StopAllCoroutines();
    }

    private void Collapse()
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            Cell currentCell = TakeCellWithSmallestEntropy();
            if (currentCell == null) break;

            currentCell.CollapseCell();

            PropagateChangesToOthers(currentCell);

            _currentCubePos = new Vector3(currentCell.X, currentCell.Y, currentCell.Z) * _cellScale;

            if (currentCell.Samples.Count > 0)
            {
                SpawnSample currentCellSpawnSample = currentCell.Samples[0];

                if (currentCellSpawnSample != null && currentCellSpawnSample.SpawnPrefab != null)
                {
                    GameObject SpawnedCell = Instantiate(currentCellSpawnSample.SpawnPrefab,
                                             (new Vector3(currentCell.X, currentCell.Y, currentCell.Z) - Vector3.up / 2) * _cellScale,
                                             Quaternion.Euler(0.0f, currentCellSpawnSample.OffsetAngle, 0.0f),
                                             transform); ;

                    Debug.Log(SpawnedCell.name);

                    SpawnedCell.transform.localScale *= _cellScale;
                }
            }
        }
        _startedCollapse = false;
    }

    /// <summary>
    /// Propagates changes in states of cells to other cells
    /// </summary>
    /// <param name="fromCell">cell that propagates to its neighbours</param>
    private void PropagateChangesToOthers(Cell fromCell)
    {
        _stackedCells.Push(fromCell);

        while (_stackedCells.Count > 0)
        {
            Cell currentCell = _stackedCells.Pop();

            Cell[] neighbours = TakeCellsNeighbours(currentCell);
            _currentNeighbourPos = new Vector3(-1000, -1000, -1000) * _cellScale;

            for (int i = 0; i < neighbours.Length; i++)
            {
                if (currentCell.Samples.Count == 0 && i != 2)
                    continue;

                Cell currentNeighbour = neighbours[i];
                if (currentNeighbour == null)
                    continue;

                _currentNeighbourPos = new Vector3(currentNeighbour.X, currentNeighbour.Y, currentNeighbour.Z) * _cellScale;

                SpawnSample[] neighboursSamples = currentNeighbour.Samples.ToArray();

                SpawnSample[] possibleNeighbours = GetPossibleNeighbours(currentCell, (SpawnSample.Axis)i);

                if (neighboursSamples.Length == 0)
                    continue;

                for (int k = 0; k < neighboursSamples.Length; k++)
                {
                    if (!possibleNeighbours.Contains(neighboursSamples[k]))
                    {
                        currentNeighbour.RemoveSample(neighboursSamples[k]);
                        if (!_stackedCells.Contains(currentNeighbour))
                            _stackedCells.Push(currentNeighbour);
                    }
                }
            }
        }
    }
    private SpawnSample[] GetPossibleNeighbours(Cell fromCell, SpawnSample.Axis axis)
    {
        List<SpawnSample> samples = new List<SpawnSample>();
        List<SpawnSample> cellSamples = fromCell.Samples;
        switch (axis)
        {
            case SpawnSample.Axis.PositiveX:
                for (int i = 0; i < cellSamples.Count; i++)
                {
                    if (cellSamples[i].XFaceAllowed != null)
                        samples.AddRange(cellSamples[i].XFaceAllowed);
                }
                break;
            case SpawnSample.Axis.NegativeX:
                for (int i = 0; i < cellSamples.Count; i++)
                {
                    if (cellSamples[i].NegativeXFaceAllowed != null)
                        samples.AddRange(cellSamples[i].NegativeXFaceAllowed);
                }
                break;
            case SpawnSample.Axis.PositiveY:
                for (int i = 0; i < cellSamples.Count; i++)
                {
                    if (cellSamples[i].YFaceAllowed != null)
                        samples.AddRange(cellSamples[i].YFaceAllowed);
                }
                break;
            case SpawnSample.Axis.NegativeY:
                for (int i = 0; i < cellSamples.Count; i++)
                {
                    if (cellSamples[i].NegativeYFaceAllowed != null)
                        samples.AddRange(cellSamples[i].NegativeYFaceAllowed);
                }
                break;
            case SpawnSample.Axis.PositiveZ:
                for (int i = 0; i < cellSamples.Count; i++)
                {
                    if (cellSamples[i].ZFaceAllowed != null)
                        samples.AddRange(cellSamples[i].ZFaceAllowed);
                }
                break;
            case SpawnSample.Axis.NegativeZ:
                for (int i = 0; i < cellSamples.Count; i++)
                {
                    if (cellSamples[i].NegativeZFaceAllowed != null)
                        samples.AddRange(cellSamples[i].NegativeZFaceAllowed);
                }
                break;
            default:
                samples = null;
                break;
        }

        return samples.ToArray();
    }

    /// <summary>
    /// Returns neighbours in following order: positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ
    /// </summary>
    /// <param name="cell">cell which neighbours to get</param>
    /// <returns></returns>
    private Cell[] TakeCellsNeighbours(Cell cell)
    {
        Cell[] neighbours = new Cell[6];

        neighbours[0] = TakeCellByPosition(cell.X + 1, cell.Y,     cell.Z);     // forward
        neighbours[1] = TakeCellByPosition(cell.X - 1, cell.Y,     cell.Z);     // backward
        neighbours[2] = TakeCellByPosition(cell.X,     cell.Y + 1, cell.Z);     // top
        neighbours[3] = TakeCellByPosition(cell.X,     cell.Y - 1, cell.Z);     // bottom
        neighbours[4] = TakeCellByPosition(cell.X,     cell.Y,     cell.Z + 1); // right
        neighbours[5] = TakeCellByPosition(cell.X,     cell.Y,     cell.Z - 1); // left

        return neighbours;
    }

    /// <summary>
    /// gets a cell with the least amount of possible samples(a.k.a. the lowest entropy)
    /// </summary>
    /// <returns>cell with the least amount of entropy</returns>
    private Cell TakeCellWithSmallestEntropy()
    {
        int minimumEntropy = int.MaxValue;
        int cellIndex = -1;

        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
        Cell[] shuffledArray = _cells.OrderBy(x => rnd.Next()).ToArray();

        for (int i = 0; i < shuffledArray.Length; i++)
        {
            if (shuffledArray[i].Samples.Count < minimumEntropy  && shuffledArray[i].Samples.Count > 0 && !shuffledArray[i].Collapsed)
            {
                minimumEntropy = shuffledArray[i].Samples.Count;
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

    /// <summary>
    /// transform coordinates into index in the cell array
    /// </summary>
    /// <returns></returns>
    private int FromPositionToIndex(int x, int y, int z)
    {
        return x + z * _gridWidth + y * (_gridWidth * _gridLength);
    }

    /// <summary>
    /// gets cell by its position in WFC solver space
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// ugly code pretty gizmos, dont judge
    /// </summary>
    private void OnDrawGizmos()
    {
        Color transparentYellow = new Color(1, 1, 0, 0.01f);
        Color transparentRed = new Color(1, 0, 0, 0.01f);

        for (int i = 0; i < _cells.Count; i++)
        {
            Gizmos.color = Color.yellow;

            if (_cells[i].Collapsed)
                Gizmos.color = Color.red;

            Vector3 cellPos = new Vector3(_cells[i].X, _cells[i].Y, _cells[i].Z) * _cellScale;
            Gizmos.DrawWireCube(cellPos, Vector3.one * _cellScale);

            Gizmos.color = transparentYellow;
            Gizmos.DrawCube(_currentCubePos, Vector3.one * _cellScale);

            Gizmos.color = transparentRed;
            Gizmos.DrawCube(_currentNeighbourPos, Vector3.one * _cellScale);

            Gizmos.color = Color.blue;
        }
    }

    public void LoadSamplesFromXML(string path)
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(path);

        XmlElement xRoot = xDoc.DocumentElement;

        foreach (XmlNode xNode in xRoot)
        {
            if (xNode.Attributes.Count > 0)
            {
                for (int i = 0; i < xNode.ChildNodes.Count; i++)
                {
                    XmlNode childNode = xNode.ChildNodes[i]; // worksheet node

                    int sampleCounter = -1;

                    for (int k = 0; k < childNode.ChildNodes.Count; k++)
                    {
                        XmlNode childChildNode = childNode.ChildNodes[k]; // table node

                        if (childChildNode.Name == "Row")
                        {
                            List<List<SpawnSample>> listOfSpawnSamples = new List<List<SpawnSample>>(6); // for one block: x, negx, y, negy, z, negz

                            for (int j = 1; j < childChildNode.ChildNodes.Count; j++)
                            {
                                string cellValue = "";
                                XmlNode cellNode = childChildNode.ChildNodes[j]; // cell node

                                if (cellNode != null && cellNode.FirstChild != null)
                                    cellValue = cellNode.FirstChild.FirstChild.Value;

                                if (cellValue == "PositiveX" || cellValue == "NegativeX" ||
                                    cellValue == "PositiveY" || cellValue == "NegativeY" ||
                                    cellValue == "PositiveZ" || cellValue == "NegativeZ")
                                {
                                    sampleCounter = -2;
                                    break;
                                }

                                string[] currentAxisSamplesStr = cellValue.Split(',');

                                listOfSpawnSamples.Add(new List<SpawnSample>());
                                    
                                if (currentAxisSamplesStr[0] != "")
                                {
                                    listOfSpawnSamples[j - 1].AddRange(new List<SpawnSample>(GetSpawnSamplesByIndices(currentAxisSamplesStr, -1)));
                                }
                                else
                                {
                                    listOfSpawnSamples[j - 1].Add(null);
                                }
                                
                            }

                            if (sampleCounter == -2)
                            {
                                sampleCounter = 0;
                                continue;
                            }

                            _spawnSamples[sampleCounter].FillSamples(listOfSpawnSamples[0].ToArray(), listOfSpawnSamples[1].ToArray(),
                                                                     listOfSpawnSamples[2].ToArray(), listOfSpawnSamples[3].ToArray(),
                                                                     listOfSpawnSamples[4].ToArray(), listOfSpawnSamples[5].ToArray());
                            sampleCounter++;
                        }
                    }
                }
            }
        }
    }

    private SpawnSample[] GetSpawnSamplesByIndices(string[] sampleIndices, int offset = 0)
    {
        if (sampleIndices.Contains("")) // check if contains empty strings
        {
            return new SpawnSample[] { null };
        }

        SpawnSample[] samples = new SpawnSample[sampleIndices.Length];

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = _spawnSamples[int.Parse(sampleIndices[i]) + offset];
        }

        return samples;
    }

    public void ClearBlocks()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
