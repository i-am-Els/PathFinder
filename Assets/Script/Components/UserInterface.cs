using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Components
{
    public class UserInterface : MonoBehaviour
    {
        [Tooltip("Dropdown: Select Action Mode.")]
        public TMP_Dropdown dropdown;
        [Tooltip("Reset/Restart simulation.")]
        public Button resetSim;
        [Tooltip("Pause simulation.")]
        public Button playAndPauseSim;
        
        [SerializeField] private Sprite playSimSprite;
        [SerializeField] private Sprite pauseSimSprite;

        public Action<Button, Sprite> PlayAndPauseSimTrigger;
        public Action ResetSimTrigger;
        public Action<int> SetActionModeTrigger;

        private void OnEnable()
        {
            resetSim.onClick.AddListener(ResetSimulation);
            playAndPauseSim.onClick.AddListener(PlayAndPauseSimulation);
            dropdown.onValueChanged.AddListener(SetActionMode);
        }

        private void Start()
        {
            dropdown.options.Clear();
            dropdown.AddOptions(new List<string>{"Add Obstacle", "Remove Obstacle"});
            dropdown.value = 0;
        }

        private void OnDisable()
        {
            resetSim.onClick.RemoveListener(ResetSimulation);
            playAndPauseSim.onClick.RemoveListener(PlayAndPauseSimulation);
            dropdown.onValueChanged.RemoveListener(SetActionMode);
            dropdown.options.Clear();
        }

        private void ResetSimulation()
        {
            ResetSimTrigger.Invoke();
        }

        private void PlayAndPauseSimulation()
        {
            PlayAndPauseSimTrigger.Invoke(playAndPauseSim, GameSession.GameIsPaused() ? pauseSimSprite : playSimSprite);
        }

        private void SetActionMode(int option)
        {
            SetActionModeTrigger.Invoke(option);
        }
    }
}