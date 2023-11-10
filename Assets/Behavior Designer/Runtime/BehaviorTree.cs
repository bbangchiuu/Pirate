using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    // Wrapper for the Behavior class
    [AddComponentMenu("Behavior Designer/Behavior Tree")]
    public class BehaviorTree : Behavior
    {
        // intentionally left blank

        private float m_ThinkingTimeScale = 1f;
        public float ThinkingTimeScale
        {
            get { return m_ThinkingTimeScale; }
            set { m_ThinkingTimeScale = value; }
        }
    }
}