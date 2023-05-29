using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnSample", menuName = "ScriptableObjects/SpawnSample", order = 1)]
public class SpawnSample : ScriptableObject
{
    [SerializeField]
    private GameObject _spawnPrefab;

    [Header("Relations")]
    [SerializeField]
    private AttachedObject[] _XFaceAllowed;
    [SerializeField]
    private AttachedObject[] _NegativeXFaceAllowed;
    [SerializeField]
    private AttachedObject[] _YFaceAllowed;
    [SerializeField]
    private AttachedObject[] _NegativeYFaceAllowed;
    [SerializeField]
    private AttachedObject[] _ZFaceAllowed;
    [SerializeField]
    private AttachedObject[] _NegativeZFaceAllowed;


    // getters
    public GameObject SpawnPrefab => _spawnPrefab;

    public AttachedObject[] XFaceAllowed => _XFaceAllowed;
    public AttachedObject[] NegativeXFaceAllowed => _NegativeXFaceAllowed;
    public AttachedObject[] YFaceAllowed => _YFaceAllowed;
    public AttachedObject[] NegativeYFaceAllowed => _NegativeYFaceAllowed;
    public AttachedObject[] ZFaceAllowed => _ZFaceAllowed;
    public AttachedObject[] NegativeZFaceAllowed => _NegativeZFaceAllowed;
}

[Serializable]
public class AttachedObject
{
    [SerializeField]
    private SpawnSample _sample;
    [SerializeField]
    private float _atAnAngle;

    public SpawnSample Sample => _sample;
    public float AtAnAngle => _atAnAngle;
}
