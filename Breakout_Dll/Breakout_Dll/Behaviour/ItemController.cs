using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Breakout.GameMain;

namespace Breakout.Behaviour
{
    public enum ItemType
    {
        LengthenBoard = 0,
        ShortenBoard,
        FireBullet
    }

    /// <summary>
    /// MonoBehavior attached to the item
    /// </summary>
    public class ItemController : MonoBehaviour
    {
        public ItemType m_itemType;
        
        private float   m_activeRangeX, m_activeRangeY;  
        private SpriteRenderer m_spriteRenderer;   

        void Awake()
        {           
            m_spriteRenderer = GetComponent<SpriteRenderer>();            
            m_activeRangeX = Camera.main.aspect * Camera.main.orthographicSize - m_spriteRenderer.bounds.extents.x;
            m_activeRangeY = Camera.main.orthographicSize - m_spriteRenderer.bounds.extents.y; 
        }      

        void Update()
        {
            Vector3 cameraPosition = Camera.main.transform.position;            
            float yMin = cameraPosition.y - m_activeRangeY;

            //if item crosses the bottom line, deactivate it.
            if (transform.position.y < yMin)
            {
                gameObject.SetActive(false);
                GameWorld.GetInstance().LevelManager.ProcessItemLost();             
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Board"))
            {
                if (m_itemType == ItemType.LengthenBoard)
                {
                    GameWorld.GetInstance().LevelManager.ChangeCurBoard(true);
                    GameWorld.GetInstance().AudioController.Play(SoundType.LevelUp);
                }
                else if (m_itemType == ItemType.ShortenBoard)
                {
                    GameWorld.GetInstance().LevelManager.ChangeCurBoard(false);
                    GameWorld.GetInstance().AudioController.Play(SoundType.LevelDown);
                }
                else if (m_itemType == ItemType.FireBullet)
                {
                    GameWorld.GetInstance().LevelManager.FireBullet();
                    GameWorld.GetInstance().AudioController.Play(SoundType.GetAmmo);
                }

                gameObject.SetActive(false);
                GameWorld.GetInstance().LevelManager.ProcessItemLost();
            }
        }
    }
}
