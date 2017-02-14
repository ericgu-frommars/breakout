using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Breakout.GameMain
{
    /// <summary>
    /// Entrance of the game
    /// </summary>
    public class GameMain : MonoBehaviour
    {
        void Start()
        {
            GameWorld.GetInstance().Init();
        }

        void OnDestroy()
        {
            GameWorld.GetInstance().Destroy();
        }

        void Update()
        {
            GameWorld.GetInstance().Update();
        }

        void LateUpdate()
        {
            GameWorld.GetInstance().LateUpdate();
        }
    }
}
