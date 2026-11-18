using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class ShipSelectionButton : MonoBehaviour
    {
        [SerializeField] private MainMenu m_MainMenu;
        [SerializeField] private SpaceShip m_Prefab;
        [SerializeField] private Text m_ShipName;
        [SerializeField] private Text m_Hitpoints;
        [SerializeField] private Text m_Speed;
        [SerializeField] private Text m_Agillity;
        [SerializeField] private Image m_Preview;

        private void Start()
        {
            if (m_Prefab == null) return;

            if (m_ShipName != null) m_ShipName.text = m_Prefab.NickName;
            if (m_Hitpoints != null) m_Hitpoints.text = "HP : " + m_Prefab.MaxHitPoints.ToString();
            if (m_Speed != null) m_Speed.text = "Speed : " + m_Prefab.MaxLinearVelocity.ToString();
            if (m_Agillity != null) m_Agillity.text = "Agillity : " + m_Prefab.MaxAngularVelocity.ToString();
            if (m_Preview != null) m_Preview.sprite = m_Prefab.PreviewImage;
        }

        public void SelectShip()
        {
            Player.SelectedSpaceShip = m_Prefab;

            if (m_MainMenu != null)
                m_MainMenu.ShowMainPanel();
        }
    }
}