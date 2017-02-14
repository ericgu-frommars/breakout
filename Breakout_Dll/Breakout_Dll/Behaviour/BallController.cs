using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Breakout.GameMain;
using Breakout.Manager;

namespace Breakout.Behaviour
{
    /// <summary>
    /// MonoBehavior attached to the ball
    /// </summary>
    public class BallController : MonoBehaviour
    {
        private Vector3 m_initialDirection;
        private Vector3 m_moveDirection;
        private float   m_moveSpeed;

        private float   m_activeRangeX, m_activeRangeY;  
        private SpriteRenderer m_spriteRenderer;        

        void Awake()
        {           
            m_spriteRenderer = GetComponent<SpriteRenderer>();            
            m_activeRangeX = Camera.main.aspect * Camera.main.orthographicSize - m_spriteRenderer.bounds.extents.x;
            m_activeRangeY = Camera.main.orthographicSize - m_spriteRenderer.bounds.extents.y; 
        }

        void OnEnable()
        {
            m_initialDirection = new Vector3(1.0f, 1.0f, 0.0f);
            m_initialDirection.Normalize();

            //
            m_moveDirection = m_initialDirection;
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            m_moveSpeed = moveSpeed;
        }
    
        void Update()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = currentPosition + m_moveDirection * m_moveSpeed;
            //transform.position = targetPosition;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime);     

            ConstrainBounds();
        }

        void ConstrainBounds()
	    {
		    Vector3 updatedPosition = transform.position;
            Vector3 cameraPosition = Camera.main.transform.position;

            //
            float xMin = cameraPosition.x - m_activeRangeX;
            float xMax = cameraPosition.x + m_activeRangeX;

            if (updatedPosition.x < xMin || updatedPosition.x > xMax) 
            {
                updatedPosition.x = Mathf.Clamp(updatedPosition.x, xMin, xMax);
                m_moveDirection.x = -m_moveDirection.x;
		    }

		    //
            float yMin = cameraPosition.y - m_activeRangeY;
            float yMax = cameraPosition.y + m_activeRangeY;

            if (updatedPosition.y > yMax) 
            {
                updatedPosition.y = Mathf.Clamp(updatedPosition.y, yMin, yMax);
			    m_moveDirection.y = -m_moveDirection.y;                
		    }

            transform.position = updatedPosition;

            //If ball crosses the bottom line, deactivate it.
            if (updatedPosition.y < yMin)
            {
                GameWorld.GetInstance().LevelManager.ProcessBallLost();
                gameObject.SetActive(false);                
            }
	    }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Tile"))
            {
                if (collision.contacts.Length > 0)
                {
                    Vector2 normal = collision.contacts[0].normal;
                    m_moveDirection = Vector3.Reflect(m_moveDirection, new Vector3(normal.x, normal.y, 0.0f));
                    m_moveDirection.Normalize();

                    if (Mathf.Abs(m_moveDirection.x) < 0.2f || Mathf.Abs(m_moveDirection.y) < 0.2f) 
                    {
                        m_moveDirection = m_initialDirection;
                    }
                }
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {             
            if (other.CompareTag("Board"))
            {
                Vector3 updatedPosition = transform.position;
                updatedPosition.y = other.transform.position.y + m_spriteRenderer.bounds.extents.y * 2;
                transform.position = updatedPosition;

                m_moveDirection.y = -m_moveDirection.y;
            }
        }
    }
}
