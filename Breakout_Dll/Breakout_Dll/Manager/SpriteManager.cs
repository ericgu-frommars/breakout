using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Breakout.Behaviour;

namespace Breakout.Manager
{
    /// <summary>
    /// It's a simple storer for Sprite
    /// </summary>
    public class SpriteManager
    {
        Dictionary<string, SpriteController> m_spriteDict = new Dictionary<string, SpriteController>();

        public Sprite GetSprite(string spritePath)
        {
            if (m_spriteDict.ContainsKey(spritePath))
            {
                return m_spriteDict[spritePath].m_sprite;
            }
            else
            {
                GameObject spritePrefab = Resources.Load<GameObject>(spritePath);
                SpriteController spriteController = spritePrefab.GetComponent<SpriteController>();
                m_spriteDict.Add(spritePath, spriteController);
                return spriteController.m_sprite;
            }
        }
    }
}
