using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Breakout.DataLoader;
using Breakout.Manager;
using Breakout.Behaviour;
using Breakout.UI;

namespace Breakout.GameMain
{
    public enum GameStage
    {
        MainMenu = 0,
        Gaming,
        GameComplete,
        GameFailed
    }

    /// <summary>
    /// It's a container of all the objects in the game
    /// </summary>
    public sealed class GameWorld
    {
        private static readonly GameWorld m_instance = new GameWorld();

        private UIController    m_uiController;
        private AudioController m_audioController;
        private VFXController   m_vfxController;
        private XmlDataLoader   m_xmlDataLoader;
        private LevelManager    m_levelManager;
        private SpriteManager   m_spriteManager;

        public static GameWorld GetInstance()
        {
            return m_instance;
        }

        public UIController UIController
        {
            get { return m_uiController; }
        }

        public AudioController AudioController
        {
            get { return m_audioController; }
        }

        public VFXController VFXController
        {
            get { return m_vfxController; }
        }

        public XmlDataLoader XmlDataLoader
        {
            get { return m_xmlDataLoader; }
        }

        public LevelManager LevelManager
        {
            get { return m_levelManager; }
        }

        public SpriteManager SpriteManager
        {
            get { return m_spriteManager; }
        }

        public void Init()
        {
            GameObject uiObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/UI/UI")) as GameObject;
            m_uiController = uiObj.GetComponent<UIController>();

            GameObject audioControllerObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/AudioController/AudioController")) as GameObject;
            m_audioController = audioControllerObj.GetComponent<AudioController>();

            GameObject vfxControllerObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/VFXController/VFXController")) as GameObject;
            m_vfxController = vfxControllerObj.GetComponent<VFXController>();

            m_xmlDataLoader = new XmlDataLoader();
            m_xmlDataLoader.Init();

            m_levelManager = new LevelManager();
            m_levelManager.Init();

            m_spriteManager = new SpriteManager();

            //
            m_uiController.SetLayout(GameStage.MainMenu);
        }

        public void Destroy()
        {

        }

        public void Update()
        {
            m_levelManager.Update();
        }

        public void LateUpdate()
        {

        }

        public void SetLayout(GameStage gameStage)
        {
            if (gameStage == GameStage.MainMenu)
            {
                m_levelManager.DeactivateAll();
                m_levelManager.Init();                
                m_uiController.Init();
            }
            else if (gameStage == GameStage.Gaming)
            {
                m_levelManager.Load();
            }
            else if (gameStage == GameStage.GameComplete)
            {
                m_levelManager.DeactivateAll();
                m_levelManager.Reset();
            }
            else if (gameStage == GameStage.GameFailed)
            {
                m_levelManager.DeactivateAll();                
            }
        }
    }
}
