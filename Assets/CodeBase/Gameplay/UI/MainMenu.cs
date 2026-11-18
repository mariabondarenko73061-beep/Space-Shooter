using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_ShipSelectionPanel;
        [SerializeField] private GameObject m_LevelSelectionPanel;
        [SerializeField] private GameObject m_MainMenu;

        private void Start()
        {
            ShowMainPanel();
            transform.SetParent(null);
        }

        private void Update()
        {
            // Escape returns to the main panel from any sub-panel.
            if (Input.GetKeyDown(KeyCode.Escape))
                ShowMainPanel();
        }

        public void ShowMainPanel()
        {
            if (m_MainMenu != null) m_MainMenu.SetActive(true);
            if (m_ShipSelectionPanel != null) m_ShipSelectionPanel.SetActive(false);
            if (m_LevelSelectionPanel != null) m_LevelSelectionPanel.SetActive(false);
        }

        public void ShowShipSelectionPanel()
        {
            if (m_ShipSelectionPanel != null) m_ShipSelectionPanel.SetActive(true);
            if (m_MainMenu != null) m_MainMenu.SetActive(false);
        }

        public void ShowLevelSelection()
        {
            if (m_LevelSelectionPanel != null) m_LevelSelectionPanel.SetActive(true);
            if (m_MainMenu != null) m_MainMenu.SetActive(false);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}