using UnityEngine;
using System;

public class GameHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField, Tooltip("The starting block in game")] private  GameObject baseCube;
    [SerializeField, Tooltip("The block prefab")] private GameObject cube;
    [SerializeField, Tooltip("The falling block prefab")] private GameObject fallingCube;
    [SerializeField, Tooltip("The particles prefab")] private GameObject particles;
    [SerializeField, Tooltip("The scene camera")] private GameObject cameraObject;
    [Header("Settings")]
    [SerializeField, Range(0,1), Tooltip("The starting speed")] private float speed = 0.1f;
    [SerializeField, Tooltip("The speed increase after every block")] private float speedIncrease = 0.001f;
    [SerializeField, Range(0.1f, 0.3f), Tooltip("The offset distance of a perfect match")] private float levelBlocksOffset = 0.15f;
    private bool _instantiateRight = true;  // instantiation side
    private int _blockY = 1;                // the y of the block to be instantiated
    private bool _playerDied;               // the player died
    private Menu _menu;                     // _menu.GetStarted checks if player can start instantiate blocks
    private CubeMover _latestCube;          // the latest block that was instantiated
    private GameObject _lastStacked;        // the latest block that was stacked on the tower
    private Vector3 _newCameraPosition;     // the position of the camera after the new instantiation
    private bool _autoStart;                // auto puts first block
    #region Events
    public delegate void UpdateScore();
    public UpdateScore UpdateScoreEvent;
    public delegate void FinishGame();
    public FinishGame FinishGameEvent;
    public delegate void PlayEffect(EffectName name);
    public PlayEffect PlayEffectEvent;
    #endregion

    private void Start()
    {
        _newCameraPosition = cameraObject.transform.position;
        _lastStacked = baseCube;
        _menu = GetComponent<Menu>();
    }

    void Update()
    {
        switch (_menu.GameStarted)
        {
            case true when !_autoStart:
                PutBlock();
                _autoStart = true;
                return;
            case true when (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !_playerDied:
                PutBlock();
                break;
        }
        MoveCamera();
    }

    private void PutBlock()
    {
        StopPreviousCube();
        InstantiateCube();
        _newCameraPosition += Vector3.up;
    }
    private void MoveCamera()
    {
        cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, _newCameraPosition, Time.deltaTime);
    }

    private void StopPreviousCube()
    {
        if (_latestCube == null)
            return;

        var previousTransform = _latestCube.transform;
        var latestCubePosition = previousTransform.position;
        var lastStackedPosition = _lastStacked.transform.position;
        var lastStackedScale = _lastStacked.transform.localScale;
        var scaleX = Math.Abs(lastStackedPosition.x - latestCubePosition.x) - lastStackedScale.x;
        var scaleZ = Math.Abs(lastStackedPosition.z - latestCubePosition.z) - lastStackedScale.z;
        if (scaleX > 0 || scaleZ > 0)
        {
            _playerDied = true;
            NotStackedBlock();
            PlayEffectEvent(EffectName.EndGame);
            FinishGameEvent();
            Destroy(_latestCube.gameObject);
            return;
        }
        if ((!_instantiateRight && Math.Abs(lastStackedPosition.x - latestCubePosition.x) < levelBlocksOffset) ||
            (_instantiateRight && Math.Abs(lastStackedPosition.z - latestCubePosition.z) < levelBlocksOffset))
        {
            var sameXZ = lastStackedPosition;  // copies the position of the last stacked 
            sameXZ.y = latestCubePosition.y;         // corrects Y to played height
            previousTransform.position = sameXZ;     // centers block over last stacked   
            sameXZ.y -= previousTransform.localScale.y / 2;     // moves Vector between the two blocks
            Instantiate(particles, sameXZ, Quaternion.identity);    // creates particles at Vector
            PlayEffectEvent(EffectName.Success);
        }
        else
        {
            switch (_instantiateRight)
            {
                case false:
                    _latestCube.transform.position = 
                        new Vector3((latestCubePosition.x + lastStackedPosition.x) / 2, latestCubePosition.y, latestCubePosition.z);
                    _latestCube.transform.localScale = new Vector3(Math.Abs(scaleX), 1, previousTransform.localScale.z);
                    break;
                case true:
                    _latestCube.transform.position =
                        new Vector3(latestCubePosition.x, latestCubePosition.y, (latestCubePosition.z + lastStackedPosition.z) / 2);
                    _latestCube.transform.localScale = new Vector3(previousTransform.localScale.x, 1, Math.Abs(scaleZ));
                    break;
            }
            CreateFalling();
            PlayEffectEvent(EffectName.Fail);
        }
        UpdateScoreEvent();
        _lastStacked = _latestCube.gameObject;
        Destroy(_latestCube);
    }
    void CreateFalling()
    {
        var newPosition = _latestCube.transform.position;
        var falling = Instantiate(fallingCube, newPosition, Quaternion.identity).GetComponent<Renderer>();
        falling.material.color = _latestCube.GetColor();
        var newScale = _lastStacked.transform.localScale;
        newScale.y = 1;
        if (!_instantiateRight)
        {
            newScale.x -= _latestCube.transform.localScale.x;
            //  if in smaller X than stacked
            if (newPosition.x > _lastStacked.transform.position.x)
            {
                newPosition.x = _lastStacked.transform.position.x + _lastStacked.transform.localScale.x/2 + newScale.x/2;
            }
            else
            {
                newPosition.x = _lastStacked.transform.position.x - _lastStacked.transform.localScale.x/2 - newScale.x/2;
            }
        }
        else
        {
            newScale.z -= _latestCube.transform.localScale.z;
            //  if in smaller Z than stacked
            if (newPosition.z > _lastStacked.transform.position.z)
            {
                newPosition.z = _lastStacked.transform.position.z + _lastStacked.transform.localScale.z/2 + newScale.z/2;
            }
            else
            {
                newPosition.z = _lastStacked.transform.position.z - _lastStacked.transform.localScale.z/2 - newScale.z/2;
            }
        }
        var fallingTransform = falling.transform;
        fallingTransform.position = newPosition;
        fallingTransform.localScale = newScale;
    }
    /// <summary>
    /// Called when new block is stopped out of previous block's limits
    /// </summary>
    void NotStackedBlock()
    {
        var falling = Instantiate(fallingCube, _latestCube.transform.position, Quaternion.identity).GetComponent<Renderer>();
        falling.material.color = _latestCube.GetColor();
        falling.transform.localScale = _latestCube.transform.localScale;
    }
    private void InstantiateCube()
    {
        if (_playerDied)
            return;
        
        Vector3 startPosition;
        switch (_instantiateRight)
        { 
            case true:
                startPosition = _lastStacked.transform.position + Vector3.left *30;
                startPosition.y = _blockY;
                _latestCube = Instantiate(cube, startPosition,Quaternion.identity ).GetComponent<CubeMover>();
                _latestCube.InitMovement(Vector3.right * speed);
                _instantiateRight = false;
                break;
            case false:
                startPosition = _lastStacked.transform.position + Vector3.back *30;
                startPosition.y = _blockY;
                _latestCube = Instantiate(cube, startPosition,Quaternion.identity ).GetComponent<CubeMover>();
                _latestCube.InitMovement(Vector3.forward * speed);
                _instantiateRight = true;
                break;
        }

        var scale = _lastStacked.transform.localScale;
        scale.y = 1;
        _latestCube.transform.localScale = scale;

        speed += speedIncrease;
        _blockY++;
    }
}
