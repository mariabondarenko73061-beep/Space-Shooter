using UnityEngine;

namespace Common
{
    /// <summary>
    ///  базовый класс всех интерактивных игровых обьектов на сцене. 
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// название обьекта для пользователя. 
        /// </summary>

        [SerializeField]
        private string m_Nickname;
        public string NickName => m_Nickname;
    }
}