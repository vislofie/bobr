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

    [SerializeField]
    private float _offsetAngle = 0.0f;

    public float OffsetAngle => _offsetAngle;

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

    public void FillSamples(SpawnSample[] positiveX, SpawnSample[] negativeX,
                            SpawnSample[] positiveY, SpawnSample[] negativeY,
                            SpawnSample[] positiveZ, SpawnSample[] negativeZ)
    {
        _XFaceAllowed =         positiveX[0] == null ? null : positiveX;
        _NegativeXFaceAllowed = negativeX[0] == null ? null : negativeX;
        _YFaceAllowed =         positiveY[0] == null ? null : positiveY;
        _NegativeYFaceAllowed = negativeY[0] == null ? null : negativeY;
        _ZFaceAllowed =         positiveZ[0] == null ? null : positiveZ;
        _NegativeZFaceAllowed = negativeZ[0] == null ? null : negativeZ;
    }

}
