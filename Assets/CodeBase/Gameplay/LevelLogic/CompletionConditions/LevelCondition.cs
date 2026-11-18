using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class LevelCondition : MonoBehaviour
    {
        public virtual bool isCompleted { get; }

        /// <summary>
        /// Порядок строки условия в подсказке уровня (меньше — выше).
        /// </summary>
        public virtual int HintOrder => 0;

        /// <summary>
        /// Текст условия для подсказки, включая текущий прогресс.
        /// Пустая строка означает, что условие не выводится.
        /// </summary>
        public virtual string GetHintText()
        {
            return string.Empty;
        }
    }
}
