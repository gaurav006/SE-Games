using System;
using System.Collections.Generic;
using UnityEngine;

namespace SEGames
{
   
    public class TileBoardManager : MonoBehaviour
    {
        private const int COLS = 6, ROWS = 6;

        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private float _tileSpacing = 1.1f;
        [SerializeField] private int _seed = -1; 

        private readonly List<TileBehaviour> _pool = new();
        private readonly TileBehaviour[,] _grid = new TileBehaviour[COLS, ROWS];
        private bool _isActive;

        private static readonly Color[] COLORS =
        {
            Color.red, Color.blue, Color.green,
            Color.yellow, Color.magenta, Color.cyan
        };

        private Action<RoomStateChangedEvent> _onRoomState;

        private void Awake()
        {
            
            for (int i = 0; i < COLS * ROWS; i++)
            {
                var go = Instantiate(_tilePrefab, transform);
                go.SetActive(false);   
                _pool.Add(go.GetComponent<TileBehaviour>());
            }
        }

        private void OnEnable()
        {
            _onRoomState = OnRoomStateChanged;
            SEBus.Listen<RoomStateChangedEvent>(_onRoomState);
        }

        private void OnDisable()
        {
            SEBus.Unlisten<RoomStateChangedEvent>(_onRoomState);
        }

        private void OnRoomStateChanged(RoomStateChangedEvent e)
        {
            if (e.NewState == RoomState.PuzzleActive) Activate();
            else if (_isActive) Deactivate();
        }

        private void Activate()
        {
            _isActive = true;
            Debug.Log("[TileBoard] Activated");
            Shuffle();
        }

        private void Deactivate()
        {
            _isActive = false;
            Debug.Log("[TileBoard] Deactivated");
            ReturnAllToPool();
        }

        public void Shuffle()
        {
            if (!_isActive) return;   
            ReturnAllToPool();

            var rng = _seed >= 0 ? new System.Random(_seed) : new System.Random();
            int poolIdx = 0;

            for (int x = 0; x < COLS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    var tile = _pool[poolIdx++];                  
                    tile.transform.localPosition = new Vector3(
                        (x - (COLS - 1) * 0.5f) * _tileSpacing,
                        (y - (ROWS - 1) * 0.5f) * _tileSpacing,
                        0f);

                    tile.Init(x, y, COLORS[rng.Next(COLORS.Length)]);
                    tile.gameObject.SetActive(true);   
                    _grid[x, y] = tile;
                    _grid[x, y].name = poolIdx.ToString();// nk  update name case check the right postion ..16-03-2026:16:00
                }
            }
        }

        private void ReturnAllToPool()
        {
            foreach (var t in _pool)
            {
                t.ReturnToPool();              
                t.gameObject.SetActive(false); 
            }
            System.Array.Clear(_grid, 0, _grid.Length);
        }
    }
}
