using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SEGames
{

    public class UIRoomPanel : MonoBehaviour
    {
        public static UIRoomPanel Instance;

        [Header("Normal progression")]
        public TMP_Text _stateLabel;
        [SerializeField] private Button _proceedButton;

        [Header("Game Complete screen")]
        [SerializeField] private GameObject _gameOverPanel;   // whole overlay
        [SerializeField] private TMP_Text _gameOverLabel;   // e.g. "Room Complete!"
        [SerializeField] private Button _newGameButton;

        private Action<RoomStateChangedEvent> _onStateChanged;

        private void Awake()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            _onStateChanged = OnStateChanged;
            SEBus.Listen<RoomStateChangedEvent>(_onStateChanged);

            _proceedButton.onClick.AddListener(OnProceedClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);


            SetGameOverVisible(false);
        }



        private void OnDisable()
        {
            SEBus.Unlisten<RoomStateChangedEvent>(_onStateChanged);
            _proceedButton.onClick.RemoveListener(OnProceedClicked);
            _newGameButton.onClick.RemoveListener(OnNewGameClicked);

        }

        private void OnStateChanged(RoomStateChangedEvent e)
        {
            _stateLabel.text = e.NewState.ToString();
            Debug.Log("[UIRoomPanel] State → " + e.NewState);

            bool isComplete = e.NewState == RoomState.RoomComplete;
            _proceedButton.gameObject.SetActive(!isComplete);
            SetGameOverVisible(isComplete);
        }

        private void OnProceedClicked()
        {
            Debug.Log("[UIRoomPanel] Proceed clicked");
            SEBus.Emit(new ProceedRequestedEvent());
        }

        private void OnNewGameClicked()
        {
            Debug.Log("[UIRoomPanel] New Game clicked");

            SEBus.Emit(new NewGameRequestedEvent());
        }



        private void SetGameOverVisible(bool visible)
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(visible);

            if (_gameOverLabel != null && visible)
                _gameOverLabel.text = "Room Complete!";
        }

    }
}
