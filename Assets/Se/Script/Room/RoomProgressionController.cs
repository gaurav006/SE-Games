using System;
using UnityEngine;

namespace SEGames
{

    public class RoomProgressionController : MonoBehaviour
    {

        private const string STATE_KEY = "room.state";

        [SerializeField] private RoomState _currentState;

        private Action<PuzzleCompleteEvent> _onPuzzleComplete;
        private Action<ProceedRequestedEvent> _onProceed;
        private Action<NewGameRequestedEvent> _onNewGame;

        private void Awake()
        {

            int saved = SEState.Get<int>(STATE_KEY, (int)RoomState.PuzzleActive);
            _currentState = (RoomState)Mathf.Clamp(saved, 0, (int)RoomState.RoomComplete);
        }

        private void OnEnable()
        {
            _onPuzzleComplete = _ => TryAdvanceFrom(RoomState.PuzzleActive);
            _onProceed = _ => AdvanceState();
            _onNewGame = _ => StartNewGame();

            SEBus.Listen<PuzzleCompleteEvent>(_onPuzzleComplete);
            SEBus.Listen<ProceedRequestedEvent>(_onProceed);
            SEBus.Listen<NewGameRequestedEvent>(_onNewGame);

            BroadcastState();
        }

        private void OnDisable()
        {
            SEBus.Unlisten<PuzzleCompleteEvent>(_onPuzzleComplete);
            SEBus.Unlisten<ProceedRequestedEvent>(_onProceed);
            SEBus.Unlisten<NewGameRequestedEvent>(_onNewGame);
        }


        public void AdvanceState()
        {
            if (_currentState == RoomState.RoomComplete) return;
            _currentState = _currentState + 1;
            SaveAndBroadcast();
        }


        private void TryAdvanceFrom(RoomState expected)
        {
            if (_currentState != expected) return;
            _currentState = expected + 1;
            SaveAndBroadcast();
        }

        private void StartNewGame()
        {
            SEState.Clear();                         // wipe all PlayerPrefs keys
            _currentState = RoomState.PuzzleActive;  // reset in-memory FSM
            SaveAndBroadcast();                      // persist + broadcast to all listeners
            Debug.Log("[RoomProgression] ★ New game — reset to PuzzleActive");
        }
        private void SaveAndBroadcast()
        {
            SEState.Set(STATE_KEY, (int)_currentState);
            SEState.Flush();   // persist immediately at every transition
            BroadcastState();
        }

        private void BroadcastState()
        {
            SEBus.Emit(new RoomStateChangedEvent { NewState = _currentState });
            Debug.Log("[RoomProgression] State → " + _currentState);
            UIRoomPanel.Instance._stateLabel.text = _currentState.ToString();
        }

        private void OnApplicationPause(bool pause) { if (pause) SEState.Flush(); }
        private void OnApplicationFocus(bool focus) { if (!focus) SEState.Flush(); }
    }
}
