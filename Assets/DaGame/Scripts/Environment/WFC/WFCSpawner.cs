using UnityEngine;

public class WFCSpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnSample[] _spawnSamples;
    [SerializeField]
    private float _cellSize = 1.0f;

    [SerializeField]
    private int _gridWidth = 8;
    [SerializeField]
    private int _gridLength = 8;
    [SerializeField]
    private int _gridHeight = 3;

    private Cell[] _cells;

    private void OnValidate()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        _cells = new Cell[_gridWidth * _gridLength * _gridHeight];

        for (int i = 0; i < _cells.Length; i++)
        {
            int currentXCoord = i % _gridWidth;
            int currentYCoord = i / (_gridWidth * _gridLength);
            int currentZCoord = i / (_gridWidth + currentYCoord * (_gridWidth * _gridLength));
            

            _cells[i] = new Cell(_spawnSamples, currentXCoord, currentYCoord, currentZCoord);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _cells.Length; i++)
        {
            Gizmos.DrawWireCube(new Vector3(_cells[i].X, _cells[i].Y, _cells[i].Z), Vector3.one * _cellSize);
        }
    }
}
