using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Breakout.Behaviour
{
    enum FlashStatus
    {
        Light = 0,
        Dark
    }

    /// <summary>
    /// MonoBehavior attached to the board
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        private SpriteRenderer m_spriteRenderer;
        private Rigidbody2D m_rigidBody2D;
        private float m_slideRange;

        private Transform[] m_fireTrans = new Transform[3];
        private bool m_isFlashEnable;

        private Color m_lightColor;
        private Color m_darkColor;        

        private FlashStatus m_curFlashStatus;
        private Color m_startColor;
        private Color m_endColor;
        private float m_timeElapsed;

        void Awake()
        {
            m_fireTrans[0] = transform.FindChild("Fire1");
            m_fireTrans[1] = transform.FindChild("Fire2");
            m_fireTrans[2] = transform.FindChild("Fire3");

            m_lightColor = new Color(1.0f, 1.0f, 1.0f);
            m_darkColor = new Color(0.784f, 0.784f, 0.078f);
        }

        void Start()
        {
            Vector3 upperRightScreenPos = new Vector3(Screen.width, Screen.height, 0.0f);
            Vector3 upperRightWorldPos  = Camera.main.ScreenToWorldPoint(upperRightScreenPos);

            m_rigidBody2D = GetComponent<Rigidbody2D>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_slideRange = upperRightWorldPos.x - m_spriteRenderer.bounds.extents.x;

            m_isFlashEnable = false;
        }

        void FixedUpdate()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 boardPosition = new Vector3(mouseWorldPos.x, transform.position.y, transform.position.z);
            boardPosition.x = Mathf.Clamp(boardPosition.x, -m_slideRange, m_slideRange);
            m_rigidBody2D.MovePosition(boardPosition);
        }

        public Vector3 GetFirePos(int index)
        {
            if (m_fireTrans[index] != null)
            {
                return m_fireTrans[index].position;
            }

            return transform.position;
        }

        public void SetFlash(bool isEnable)
        {
            m_isFlashEnable = isEnable;

            if (m_isFlashEnable)
            {
                m_curFlashStatus = FlashStatus.Light;
                m_startColor = m_lightColor;
                m_endColor = m_darkColor;
                m_timeElapsed = 0.0f;
            }
            else
            {
                if (m_spriteRenderer == null)
                {
                    m_spriteRenderer = GetComponent<SpriteRenderer>();
                }

                m_spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }

        void Update()
        {
            if (!m_isFlashEnable)
                return;

            m_timeElapsed += Time.deltaTime;
            m_spriteRenderer.color = Color.Lerp(m_startColor, m_endColor, (float)(m_timeElapsed / 0.8f));

            if (m_timeElapsed >= 0.8f)
            {
                if (m_curFlashStatus == FlashStatus.Light)
                {
                    m_curFlashStatus = FlashStatus.Dark;
                    m_startColor = m_darkColor;
                    m_endColor = m_lightColor;
                }
                else if (m_curFlashStatus == FlashStatus.Dark)
                {
                    m_curFlashStatus = FlashStatus.Light;
                    m_startColor = m_lightColor;
                    m_endColor = m_darkColor;
                }

                m_timeElapsed = 0.0f;
            }
        }      
    }
}
