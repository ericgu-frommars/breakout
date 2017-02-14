using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Breakout.GameMain;

namespace Breakout.UI
{
    /// <summary>
    /// Control the display of UI
    /// </summary>
    public class UIController : MonoBehaviour
    {
        public Text   m_scoreText;
        public Text   m_levelText;

        public Image  m_splashImage;
        public Image  m_gameoverImage;
        public Image  m_readyImage;
        public Image  m_goImage;
        public Image  m_levelCompleteImage;

        public Button m_startBtn;
        public Button m_restartBtn;

        public GameObject m_comboPanel;
        public Image m_comboImage1;
        public Image m_comboImage2;

        void Awake()
        {
            m_startBtn.onClick.AddListener(onStartBtnClicked);
            m_restartBtn.onClick.AddListener(onRestartBtnClicked);
        }
        
        void onStartBtnClicked()
        {
            SetLayout(GameStage.Gaming);
        }

        void onRestartBtnClicked()
        {
            SetLayout(GameStage.MainMenu);
        }

        public void Init()
        {
            m_scoreText.text = "Score:0";
            m_levelText.text = "";
        }

        public void SetScoreText(string scoreText)
        {
            m_scoreText.text = "Score:" + scoreText;
        }

        public void SetLevelText(string levelText)
        {
            m_levelText.text = levelText;
        }

        public void SetLayout(GameStage gameStage)
        {
            StartCoroutine(SetLayoutProcess(gameStage));
        }

        IEnumerator SetLayoutProcess(GameStage gameStage)
        {
            if (gameStage == GameStage.MainMenu)
            {
                m_scoreText.gameObject.SetActive(false);
                m_levelText.gameObject.SetActive(false);

                m_splashImage.gameObject.SetActive(true);
                m_gameoverImage.gameObject.SetActive(false);
                m_readyImage.gameObject.SetActive(false);
                m_goImage.gameObject.SetActive(false);
                m_levelCompleteImage.gameObject.SetActive(false);

                m_startBtn.gameObject.SetActive(true);
                m_restartBtn.gameObject.SetActive(false);
                m_comboPanel.gameObject.SetActive(false);

                //
                GameWorld.GetInstance().SetLayout(GameStage.MainMenu);
            }
            else if (gameStage == GameStage.Gaming)
            {
                m_scoreText.gameObject.SetActive(false);
                m_levelText.gameObject.SetActive(false);

                m_splashImage.gameObject.SetActive(false);
                m_gameoverImage.gameObject.SetActive(false);

                m_levelCompleteImage.gameObject.SetActive(false);
                m_startBtn.gameObject.SetActive(false);
                m_restartBtn.gameObject.SetActive(false);
                m_comboPanel.gameObject.SetActive(false);

                //Display Ready and Go dynamically.
                yield return new WaitForSeconds(1.0f);
                m_readyImage.gameObject.SetActive(true);

                yield return new WaitForSeconds(1.0f);
                m_readyImage.gameObject.SetActive(false);

                yield return new WaitForSeconds(1.0f);
                m_goImage.gameObject.SetActive(true);

                yield return new WaitForSeconds(1.0f);
                m_goImage.gameObject.SetActive(false);

                yield return new WaitForSeconds(1.0f);
                
                m_scoreText.gameObject.SetActive(true);
                m_levelText.gameObject.SetActive(true);

                //
                GameWorld.GetInstance().SetLayout(GameStage.Gaming);
            }
            else if (gameStage == GameStage.GameComplete)
            {
                GameWorld.GetInstance().SetLayout(GameStage.GameComplete);

                m_scoreText.gameObject.SetActive(false);
                m_levelText.gameObject.SetActive(false);
                m_comboPanel.gameObject.SetActive(false);

                yield return new WaitForSeconds(1.0f);
                m_levelCompleteImage.gameObject.SetActive(true);

                yield return new WaitForSeconds(1.0f);
                m_levelCompleteImage.gameObject.SetActive(false);

                SetLayout(GameStage.Gaming);
            }
            else if (gameStage == GameStage.GameFailed)
            {
                GameWorld.GetInstance().SetLayout(GameStage.GameFailed);

                m_splashImage.gameObject.SetActive(false);                
                m_readyImage.gameObject.SetActive(false);
                m_goImage.gameObject.SetActive(false);
                m_levelCompleteImage.gameObject.SetActive(false);
                m_startBtn.gameObject.SetActive(false);
                m_comboPanel.gameObject.SetActive(false);

                yield return new WaitForSeconds(1.0f);
                m_gameoverImage.gameObject.SetActive(true);

                yield return new WaitForSeconds(1.0f);
                m_restartBtn.gameObject.SetActive(true);
            }
        }

        public void HideComboPanel()
        {
            m_comboPanel.SetActive(false);
        }

        public void DisplayCombo(int comboTime)
        {
            if (comboTime < 2)
                return;

            m_comboPanel.SetActive(true);

            if (comboTime < 10)
            {
                m_comboImage1.gameObject.SetActive(false);
                m_comboImage2.gameObject.SetActive(true);
                m_comboImage2.sprite = GameWorld.GetInstance().SpriteManager.GetSprite("Prefab/UI/Combo/" + comboTime.ToString());                    
            }
            else
            {
                m_comboImage1.gameObject.SetActive(true);
                m_comboImage2.gameObject.SetActive(true);
                m_comboImage1.sprite = GameWorld.GetInstance().SpriteManager.GetSprite("Prefab/UI/Combo/" + (comboTime / 10).ToString());
                m_comboImage2.sprite = GameWorld.GetInstance().SpriteManager.GetSprite("Prefab/UI/Combo/" + (comboTime % 10).ToString());
            }
        }
    }
}
