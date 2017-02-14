using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Breakout.Behaviour
{
    /// <summary>
    /// MonoBehavior attached to the bullet
    /// </summary>
    public class BulletController : MonoBehaviour
    {
        private Vector3 m_moveDirection;
        private float   m_moveSpeed;
        private float   m_activeRangeX, m_activeRangeY;  
        private SpriteRenderer m_spriteRenderer;

        void Start()
        {           
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_activeRangeX = Camera.main.aspect * Camera.main.orthographicSize - m_spriteRenderer.bounds.extents.x;
            m_activeRangeY = Camera.main.orthographicSize - m_spriteRenderer.bounds.extents.y; 
        }

        void OnEnable()
        {
            m_moveDirection = new Vector3(0.0f, 1.0f, 0.0f);
            m_moveSpeed = 9.0f;
        }

        void Update()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = currentPosition + m_moveDirection * m_moveSpeed;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime);     

            DetectLimit();
        }

        void DetectLimit()
	    {
		    Vector3 curPos = transform.position;
            Vector3 cameraPosition = Camera.main.transform.position;

            //
            float xMin = cameraPosition.x - m_activeRangeX;
            float xMax = cameraPosition.x + m_activeRangeX;
            float yMin = cameraPosition.y - m_activeRangeY;
            float yMax = cameraPosition.y + m_activeRangeY;

            if (curPos.x < xMin || curPos.x > xMax || curPos.y < yMin || curPos.y > yMax)
            {
                gameObject.SetActive(false);                
		    }
	    }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Tile"))
            {
                gameObject.SetActive(false); 
            }
        }
    }
}
