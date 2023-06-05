using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnSample", menuName = "ScriptableObjects/SpawnSample", order = 1)]
public class SpawnSample : ScriptableObject
{
    public enum Axis
    {
        PositiveX = 0, NegativeX, PositiveY, NegativeY, PositiveZ, NegativeZ
    }

    [SerializeField]
    private GameObject _spawnPrefab;

    [Range(0f, 1f)]
    [SerializeField]
    private float _randomChance = 1.0f;

    [Header("Relations")]
    [SerializeField]
    private SpawnSample[] _XFaceAllowed;
    [SerializeField]
    private SpawnSample[] _NegativeXFaceAllowed;
    [SerializeField]
    private SpawnSample[] _YFaceAllowed;
    [SerializeField]
    private SpawnSample[] _NegativeYFaceAllowed;
    [SerializeField]
    private SpawnSample[] _ZFaceAllowed;
    [SerializeField]
    private SpawnSample[] _NegativeZFaceAllowed;


    public GameObject SpawnPrefab => _spawnPrefab;

    public SpawnSample[] XFaceAllowed => _XFaceAllowed;
    public SpawnSample[] NegativeXFaceAllowed => _NegativeXFaceAllowed;
    public SpawnSample[] YFaceAllowed => _YFaceAllowed;
    public SpawnSample[] NegativeYFaceAllowed => _NegativeYFaceAllowed;
    public SpawnSample[] ZFaceAllowed => _ZFaceAllowed;
    public SpawnSample[] NegativeZFaceAllowed => _NegativeZFaceAllowed;
    public float RandomChance => _randomChance;

    /// <summary>
    /// Returns all neighbouring cell rules in following order: positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ
    /// </summary>
    public List<SpawnSample[]> AllowedNeighbours
    {
        get
        {
            return new List<SpawnSample[]> { _XFaceAllowed, _NegativeXFaceAllowed, _YFaceAllowed, _NegativeYFaceAllowed, _ZFaceAllowed, _NegativeZFaceAllowed };
        }
    }
}
