using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Breakout.GameMain;

namespace Breakout.Behaviour
{
    public enum TileType
    {
        ShellTile = 0,
        CyanTile,
        RedTile,
        YellowTile
    }

    enum FlashType
    {
        Light = 0,
        Dark
    }

    /// <summary>
    /// MonoBehavior attached to the tile
    /// </summary>
    public class TileController : MonoBehaviour
    {
        public  TileType m_tileType;
        private int m_score;
        private int m_life;

        private SpriteRenderer m_spriteRenderer;
        private BoxCollider2D m_boxCollider;

        private Color m_lightColor;
        private Color m_darkColor;
        private FlashType m_flashType;
        private Color m_startColor;
        private Color m_endColor;
        private float m_timeElapsed;

        void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_boxCollider = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
            m_lightColor = new Color(1.0f, 1.0f, 1.0f);
            m_darkColor = new Color(0.588f, 0.588f, 0.588f);

            m_flashType = FlashType.Light;
            m_startColor = m_lightColor;
            m_endColor = m_darkColor;
            m_timeElapsed = 0.0f;
        }

        void OnEnable()
        {
            m_spriteRenderer.color = Color.white;
            m_boxCollider.enabled = true;

            if (m_tileType == TileType.ShellTile)
            {
                m_score = 100;
                m_life = 2;
            }
            else if (m_tileType == TileType.CyanTile)
            {
                m_score = 50;
                m_life = 1;
            }
            else if (m_tileType == TileType.RedTile)
            {
                m_score = 70;
                m_life  = 1;
            }
            else if (m_tileType == TileType.YellowTile)
            {
                m_score = 70;
                m_life  = 1;
            }
        }

        void DisplayGray()
        {
            StartCoroutine(DisplayGrayProcess());
        }

        IEnumerator DisplayGrayProcess()
        {
            Color originalColor = m_spriteRenderer.color;

            float t = 0.0f;
            while(t < 1.0f)
            {
                t += Time.deltaTime * 2;
                m_spriteRenderer.color = Color.Lerp(originalColor, new Color(0.5f, 0.5f, 0.5f), t);
                yield return null;
            }
        }

        IEnumerator Disappear()
        {
            m_boxCollider.enabled = false;
            yield return StartCoroutine(DisappearProcess());
            gameObject.SetActive(false);
        }

        IEnumerator DisappearProcess()
        {
            Color originalColor = m_spriteRenderer.color;

            float t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime;
                m_spriteRenderer.color = Color.Lerp(originalColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), t);
                yield return null;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                if (m_life > 0)
                {
                    if (--m_life == 0)
                    {
                        StartCoroutine(Disappear());
                        GameWorld.GetInstance().LevelManager.ProcessTileDestroyed(transform.position, m_tileType, m_score);
                        GameWorld.GetInstance().AudioController.Play(SoundType.Destroy);
                    }
                    else
                    {
                        DisplayGray();
                        GameWorld.GetInstance().AudioController.Play(SoundType.Injured);
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        { 
            if (other.CompareTag("Bullet"))
            {
                if (m_life > 0)
                {
                    m_life = 0;

                    StartCoroutine(Disappear());
                    GameWorld.GetInstance().LevelManager.AddComboHitTime();
                    GameWorld.GetInstance().LevelManager.ProcessTileDestroyed(transform.position, m_tileType, m_score);                    
                    GameWorld.GetInstance().AudioController.Play(SoundType.Destroy);

                    //Play VFX
                    GameWorld.GetInstance().VFXController.DisplayVFX("Prefab/VFX/CFXM_Explosion", transform.position);
                }
            }
        }

        void Update()
        {
            m_timeElapsed += Time.deltaTime;
            m_spriteRenderer.color = Color.Lerp(m_startColor, m_endColor, (float)(m_timeElapsed / 1.5f));

            if (m_timeElapsed >= 1.5f)
            {
                if (m_flashType == FlashType.Light)
                {
                    m_flashType = FlashType.Dark;
                    m_startColor = m_darkColor;
                    m_endColor = m_lightColor;
                }
                else if (m_flashType == FlashType.Dark)
                {
                    m_flashType = FlashType.Light;
                    m_startColor = m_lightColor;
                    m_endColor = m_darkColor;
                }
                m_timeElapsed = 0.0f;
            }
        }
    }
}
