using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Breakout.Behaviour
{
    enum EnvTime
    {
        Morning = 0,
        Afternoon,
        Evening
    }

    /// <summary>
    /// Control the change of environment
    /// </summary>
    public class EnvironmentController : MonoBehaviour
    {
        public SpriteRenderer[] m_grassSpriteRenderer;
        private SpriteRenderer m_spriteRenderer;

        private Color m_morningColor;
        private Color m_afternoonColor;
        private Color m_eveningColor;

        private EnvTime m_curEnvTime;
        private Color m_startColor;
        private Color m_endColor;
        private float m_timeElapsed;

        void Start()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();

            m_morningColor = new Color(1.0f, 1.0f, 1.0f);
            m_afternoonColor = new Color(1.0f, 1.0f, 0.0f);
            m_eveningColor = new Color(0.196f, 0.196f, 0.784f);

            m_curEnvTime = EnvTime.Morning;
            m_startColor = m_morningColor;
            m_endColor = m_afternoonColor;
            m_timeElapsed = 0.0f;
        }

        void Update()
        {
            m_timeElapsed += Time.deltaTime;

            Color resultColor = Color.Lerp(m_startColor, m_endColor, (float)(m_timeElapsed / 30.0f));

            m_spriteRenderer.color = resultColor;
            for (int index = 0; index <m_grassSpriteRenderer.Length; index++)
            {
                m_grassSpriteRenderer[index].color = resultColor;
            }

            //
            if (m_timeElapsed >= 30.0f)
            {
                if (m_curEnvTime == EnvTime.Morning)
                {
                    m_curEnvTime = EnvTime.Afternoon;
                    m_startColor = m_afternoonColor;
                    m_endColor = m_eveningColor;
                }
                else if (m_curEnvTime == EnvTime.Afternoon)
                {
                    m_curEnvTime = EnvTime.Evening;
                    m_startColor = m_eveningColor;
                    m_endColor = m_morningColor;
                }
                else if (m_curEnvTime == EnvTime.Evening)
                {
                    m_curEnvTime = EnvTime.Morning;
                    m_startColor = m_morningColor;
                    m_endColor = m_afternoonColor;
                }

                m_timeElapsed = 0.0f;
            }
        }
    }
}
