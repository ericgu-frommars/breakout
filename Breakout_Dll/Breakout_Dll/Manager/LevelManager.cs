using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Breakout.GameMain;
using Breakout.DataLoader;
using Breakout.Behaviour;
using UnityEngine;

namespace Breakout.Manager
{
    public enum BoardType
    {
        Normal = 0,
        Long,
        Longest
    }

    /// <summary>
    /// Manager the execution of level
    /// </summary>
    public class LevelManager
    {
        private GameObject m_levelManagerObj;

        List<BallController> m_ballControllerList;
        List<BulletController> m_bulletControllerList;
        private Dictionary<int, List<TileController>> m_tilePoolDict;
        private Dictionary<int, List<ItemController>> m_itemPoolDict;
        private string[] m_tilePrefabName;        
        private string[] m_itemPrefabName;

        private BoardType m_curBoardType;
        private int m_totalTileCount;
        private int m_totalBallCount;
        private int m_totalItemCount;
        private int m_totalScore;
        private int m_levelID;
        private int m_levelLoopTime;
        private float m_ballSpeed;
        private bool  m_isFiring;
        private float m_firingStartTime;
        private float m_lastFiringTick;

        private int m_comboHitTime;
        private int m_maximumComboHitTime;
        private float m_comboTimeAccumulated;

        private BoardController m_boardController;
        private BoardController m_longBoardController;
        private BoardController m_longestBoardController;

        public LevelManager()
        {
            m_levelManagerObj = new GameObject("LevelManager");

            m_ballControllerList = new List<BallController>();
            m_bulletControllerList = new List<BulletController>();

            m_tilePoolDict = new Dictionary<int, List<TileController>>();
            m_itemPoolDict = new Dictionary<int, List<ItemController>>();
            m_tilePrefabName = new string[] { "Prefab/Tile/ShellTile", "Prefab/Tile/CyanTile", "Prefab/Tile/RedTile", "Prefab/Tile/YellowTile" };
            m_itemPrefabName = new string[] { "Prefab/Item/LengthenItem", "Prefab/Item/ShortenItem", "Prefab/Item/BulletItem" };

            GameObject boardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Board/Board")) as GameObject;
            m_boardController = boardObj.GetComponent<BoardController>();
            m_boardController.gameObject.SetActive(false);

            GameObject longBoardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Board/LongBoard")) as GameObject;
            m_longBoardController = longBoardObj.GetComponent<BoardController>();
            m_longBoardController.gameObject.SetActive(false);

            GameObject longestBoardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Board/LongestBoard")) as GameObject;
            m_longestBoardController = longestBoardObj.GetComponent<BoardController>();
            m_longestBoardController.gameObject.SetActive(false);

            PrevCreateTile();
        }

        public void Init()
        {
            m_curBoardType = BoardType.Normal;
            m_totalTileCount = 0;
            m_totalBallCount = 0;
            m_totalItemCount = 0;

            m_totalScore = 0;
            m_levelID = 1;
            m_levelLoopTime = 0;
            m_ballSpeed = 6.0f;
            m_isFiring = false;

            m_comboHitTime = 0;
            m_maximumComboHitTime = 0;
            m_comboTimeAccumulated = 0.0f;

            SetBoardFlash(false);
        }

        public void Reset()
        {
            m_curBoardType = BoardType.Normal;
            m_totalTileCount = 0;
            m_totalBallCount = 0;
            m_totalItemCount = 0;

            m_ballSpeed = 6.0f;
            m_isFiring = false;

            m_comboHitTime = 0;
            m_maximumComboHitTime = 0;
            m_comboTimeAccumulated = 0.0f;

            SetBoardFlash(false);
        }

        public int ComboHitTime
        {
            get { return m_comboHitTime; }
        }

        void PrevCreateTile()
        {
            for(int count = 0; count < 15; count++)
            {
                CreateTile(0, 0, (int)TileType.ShellTile);
                CreateTile(0, 0, (int)TileType.CyanTile);
                CreateTile(0, 0, (int)TileType.RedTile);
                CreateTile(0, 0, (int)TileType.YellowTile);
            }

            DeactivateAllTiles();
        }

        void DisplayBoard(BoardType boardType, bool isDisplay)
        {
            if (boardType == BoardType.Normal)
            {
                m_boardController.gameObject.SetActive(isDisplay);
            }
            else if (boardType == BoardType.Long)
            {
                m_longBoardController.gameObject.SetActive(isDisplay);
            }
            else if (boardType == BoardType.Longest)
            {
                m_longestBoardController.gameObject.SetActive(isDisplay);
            }
        }

        public void ChangeCurBoard(bool isLengthenBoard)
        {
            BoardType updatedBoardType = m_curBoardType;

            if (isLengthenBoard)
            {
                updatedBoardType++;
                if (updatedBoardType > BoardType.Longest)
                    updatedBoardType = BoardType.Longest;
            }
            else
            {
                updatedBoardType--;
                if (updatedBoardType < BoardType.Normal)
                    updatedBoardType = BoardType.Normal;
            }

            if (m_curBoardType != updatedBoardType)
            {
                DisplayBoard(m_curBoardType, false);
                DisplayBoard(updatedBoardType, true);
                m_curBoardType = updatedBoardType;

                /*
                if (!isLengthenBoard)
                {
                    ChangeBallSpeed(false);
                }*/
            }
        }

        public void FireBullet()
        {
            m_isFiring = true;
            m_firingStartTime = Time.time;
            m_lastFiringTick = 0.0f;

            SetBoardFlash(true);
        }

        void DisableFireBullet()
        {
            m_isFiring = false;
            SetBoardFlash(false);
        }

        void SetBoardFlash(bool isEnable)
        {
            if (m_curBoardType == BoardType.Normal)
            {
                m_boardController.SetFlash(isEnable);
            }
            else if (m_curBoardType == BoardType.Long)
            {
                m_longBoardController.SetFlash(isEnable);
            }
            else if (m_curBoardType == BoardType.Longest)
            {
                m_longestBoardController.SetFlash(isEnable);
            }
        }

        public void Update()
        {
            float curTime = Time.time;

            //Fire Bullet
            if (m_isFiring)
            {
                if (curTime - m_firingStartTime > 3.5f)
                {
                    m_isFiring = false;
                    SetBoardFlash(false);
                }
            }

            if (m_isFiring)
            {
                if (curTime - m_lastFiringTick > 0.2f)
                {
                    m_lastFiringTick = curTime;

                    if (m_curBoardType == BoardType.Normal)
                    {
                        CreateBullet(m_boardController.GetFirePos(0));
                    }
                    else if (m_curBoardType == BoardType.Long)
                    {
                        CreateBullet(m_longBoardController.GetFirePos(0));
                        CreateBullet(m_longBoardController.GetFirePos(1));
                    }
                    else if (m_curBoardType == BoardType.Longest)
                    {
                        CreateBullet(m_longestBoardController.GetFirePos(0));
                        CreateBullet(m_longestBoardController.GetFirePos(1));
                        CreateBullet(m_longestBoardController.GetFirePos(2));
                    }
                    
                    //Play Sound
                    GameWorld.GetInstance().AudioController.Play(SoundType.FireBullet);
                }
            }

            //Process Combo
            if (m_comboHitTime > 0)
            {
                m_comboTimeAccumulated += Time.deltaTime;
                if (m_comboTimeAccumulated >= 2.0f)
                {
                    m_comboHitTime = 0;
                    GameWorld.GetInstance().UIController.HideComboPanel();
                }
            }
        }

        public void AddComboHitTime()
        {
            m_comboHitTime++;
            m_comboTimeAccumulated = 0.0f;

            if (m_comboHitTime > 1 && m_comboHitTime > m_maximumComboHitTime)
            {
                m_maximumComboHitTime = m_comboHitTime;
            }

            if (m_comboHitTime > 1)
            {
                GameWorld.GetInstance().AudioController.Play(SoundType.Combo);
                GameWorld.GetInstance().UIController.DisplayCombo(m_comboHitTime);
                iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("x", 0.4f, "y", 0.4f, "time", 0.8f)); 
            }
        }
        
        public void ProcessTileDestroyed(Vector3 tilePos, TileType tileType, int score)
        {
            m_totalScore += score;
            GameWorld.GetInstance().UIController.SetScoreText(m_totalScore.ToString());

            if (tileType == TileType.CyanTile)
            {
                if (m_totalBallCount < 3)
                {
                    if (UnityEngine.Random.Range(0, 100) < 30)
                    {
                        ChangeBallSpeed(true);
                        CreateBall(tilePos + new Vector3(0.0f, 1.0f, 0.0f));
                    }
                }
            }
            else if (tileType == TileType.RedTile)
            {
                if (m_totalItemCount < 3)
                {
                    bool isItemCreated = false;

                    if (m_curBoardType < BoardType.Longest)
                    {
                        if (UnityEngine.Random.Range(0, 100) < 30)
                        {
                            CreateItem(tilePos, (int)ItemType.LengthenBoard);
                            isItemCreated = true;
                        }
                    }
                    if (!isItemCreated && !m_isFiring)
                    {
                        if (UnityEngine.Random.Range(0, 100) < 50)
                        {
                            CreateItem(tilePos, (int)ItemType.FireBullet);
                        }
                    }
                }
            }
            else if (tileType == TileType.YellowTile)
            {
                if (m_totalItemCount < 3)
                {
                    bool isItemCreated = false;

                    if (m_curBoardType > BoardType.Normal)
                    {
                        if (UnityEngine.Random.Range(0, 100) < 30)
                        {
                            CreateItem(tilePos, (int)ItemType.ShortenBoard);
                            isItemCreated = true;
                        }
                    }
                    if (!isItemCreated && !m_isFiring)
                    {
                        if (UnityEngine.Random.Range(0, 100) < 50)
                        {
                            CreateItem(tilePos, (int)ItemType.FireBullet);
                        }
                    }
                }
            }            

            if (--m_totalTileCount <= 0)
            {
                GameWorld.GetInstance().UIController.SetLayout(GameStage.GameComplete);
                DisableFireBullet();
            }
        }

        public void ProcessBallLost()
        {
            if (--m_totalBallCount <= 0)
            {
                GameWorld.GetInstance().UIController.SetLayout(GameStage.GameFailed);
                DisableFireBullet();
            }
        }

        public void ProcessItemLost()
        {
            if (--m_totalItemCount <= 0)
                m_totalItemCount = 0;
        }

        public void Load()
        {
            string displayLevelName = "Level" + (m_levelID + m_levelLoopTime * 5).ToString().PadLeft(2, '0');
            GameWorld.GetInstance().UIController.SetLevelText(displayLevelName);

            //Load Tile
            string levelName = "Level" + m_levelID.ToString().PadLeft(2, '0');
            LevelConfig levelConfig = GameWorld.GetInstance().XmlDataLoader.GetLevelConfig(levelName);
            if (levelConfig != null)
            {
                m_totalTileCount = levelConfig.m_tileDataList.Count();

                foreach (TileData tileData in levelConfig.m_tileDataList)
                {
                    CreateTile(tileData.x, tileData.y, tileData.type);
                }
            }

            //Display Board
            DisplayBoard(m_curBoardType, true);

            //Display Ball
            CreateBall(new Vector3(UnityEngine.Random.Range(-2.0f, 2.0f), 1.0f, 0.0f));

            if (++m_levelID > 5)
            {
                m_levelLoopTime++;
                m_levelID = 1;                
            }
        }

        public void DeactivateAll()
        {
            DeactivateAllTiles();
            DeactivateAllItems();
            DeactivateAllBalls();
            DeactivateAllBoards();
            DeactivateAllBullets();
        }

        void DeactivateAllTiles()
        {
            foreach (List<TileController> tileControllerList in m_tilePoolDict.Values)
            {
                foreach (TileController tileController in tileControllerList)
                {
                    //tileController.Init();
                    tileController.gameObject.SetActive(false);
                }
            }
        }

        void DeactivateAllItems()
        {
            foreach (List<ItemController> itemControllerList in m_itemPoolDict.Values)
            {
                foreach (ItemController itemController in itemControllerList)
                {
                    itemController.gameObject.SetActive(false);
                }
            }
        }

        void DeactivateAllBalls()
        {
            foreach (BallController ballController in m_ballControllerList)
            {
                ballController.gameObject.SetActive(false);
            }
        }

        void DeactivateAllBullets()
        {
            foreach (BulletController bulletController in m_bulletControllerList)
            {
                bulletController.gameObject.SetActive(false);
            }
        }

        void DeactivateAllBoards()
        {
            m_boardController.gameObject.SetActive(false);
            m_longBoardController.gameObject.SetActive(false);
            m_longestBoardController.gameObject.SetActive(false);
        }

        void ChangeBallSpeed(bool isIncrease)
        {
            if (isIncrease)
            {
                m_ballSpeed += 1.0f;
                if (m_ballSpeed > 8.0f)
                    m_ballSpeed = 8.0f;
            }
            else
            {
                m_ballSpeed -= 1.0f;
                if (m_ballSpeed < 6.0f)
                    m_ballSpeed = 6.0f;
            }

            foreach (BallController ballController in m_ballControllerList)
            {
                ballController.SetMoveSpeed(m_ballSpeed);
            }
        }

        void CreateTile(float posX, float posY, int type)
        {
            if (!m_tilePoolDict.ContainsKey(type))
            {
                GameObject tileObj = GameObject.Instantiate(Resources.Load<GameObject>(m_tilePrefabName[type]), new Vector3(posX, posY, 0.0f), Quaternion.identity) as GameObject;
                TileController tileController = tileObj.GetComponent<TileController>();
                tileObj.transform.parent = m_levelManagerObj.transform;

                List<TileController> tileControllerList = new List<TileController>();
                tileControllerList.Add(tileController);

                m_tilePoolDict.Add(type, tileControllerList);
            }
            else
            {
                List<TileController> tileControllerList = m_tilePoolDict[type];

                bool isFoundIdleTile = false;
                foreach (TileController tileController in tileControllerList)
                {
                    if (!tileController.gameObject.activeInHierarchy)
                    {
                        isFoundIdleTile = true;
                        tileController.gameObject.SetActive(true);
                        tileController.gameObject.transform.position = new Vector3(posX, posY, 0.0f);
                        break;
                    }
                }

                if (!isFoundIdleTile)
                {
                    GameObject tileObj = GameObject.Instantiate(Resources.Load<GameObject>(m_tilePrefabName[type]), new Vector3(posX, posY, 0.0f), Quaternion.identity) as GameObject;
                    TileController tileController = tileObj.GetComponent<TileController>();
                    tileObj.transform.parent = m_levelManagerObj.transform;

                    //
                    tileControllerList.Add(tileController);
                }
            }
        }

        void CreateItem(Vector3 pos, int type)
        {
            if (!m_itemPoolDict.ContainsKey(type))
            {
                GameObject itemObj = GameObject.Instantiate(Resources.Load<GameObject>(m_itemPrefabName[type]), pos, Quaternion.identity) as GameObject;
                ItemController itemController = itemObj.GetComponent<ItemController>();
                itemObj.transform.parent = m_levelManagerObj.transform;

                List<ItemController> itemControllerList = new List<ItemController>();
                itemControllerList.Add(itemController);

                m_itemPoolDict.Add(type, itemControllerList);
            }
            else
            {
                List<ItemController> itemControllerList = m_itemPoolDict[type];

                bool isFoundIdleItem = false;
                foreach (ItemController itemController in itemControllerList)
                {
                    if (!itemController.gameObject.activeInHierarchy)
                    {
                        isFoundIdleItem = true;
                        itemController.gameObject.SetActive(true);
                        itemController.gameObject.transform.position = pos;
                        break;
                    }
                }

                if (!isFoundIdleItem)
                {
                    GameObject itemObj = GameObject.Instantiate(Resources.Load<GameObject>(m_itemPrefabName[type]), pos, Quaternion.identity) as GameObject;
                    ItemController itemController = itemObj.GetComponent<ItemController>();
                    itemObj.transform.parent = m_levelManagerObj.transform;

                    itemControllerList.Add(itemController);
                }
            }

            m_totalItemCount++;
        }

        void CreateBall(Vector3 pos)
        {
            bool isFoundIdleBall = false;
            foreach (BallController ballController in m_ballControllerList)
            {
                if (!ballController.gameObject.activeInHierarchy)
                {
                    isFoundIdleBall = true;
                    ballController.gameObject.SetActive(true);
                    ballController.gameObject.transform.position = pos;
                    ballController.SetMoveSpeed(m_ballSpeed);
                    break;
                }
            }

            if (!isFoundIdleBall)
            {
                GameObject ballObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Ball/Ball"), pos, Quaternion.identity) as GameObject;
                BallController ballController = ballObj.GetComponent<BallController>();                
                ballObj.transform.parent = m_levelManagerObj.transform;
                ballController.SetMoveSpeed(m_ballSpeed);

                m_ballControllerList.Add(ballController);
            }

            m_totalBallCount++;
        }

        void CreateBullet(Vector3 pos)
        {
            bool isFoundIdleBullet = false;
            foreach (BulletController bulletController in m_bulletControllerList)
            {
                if (!bulletController.gameObject.activeInHierarchy)
                {
                    isFoundIdleBullet = true;
                    bulletController.gameObject.SetActive(true);
                    bulletController.gameObject.transform.position = pos;
                    break;
                }
            }

            if (!isFoundIdleBullet)
            {
                GameObject bulletObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Bullet/Bullet"), pos, Quaternion.identity) as GameObject;
                BulletController bulletController = bulletObj.GetComponent<BulletController>();
                bulletObj.transform.parent = m_levelManagerObj.transform;

                m_bulletControllerList.Add(bulletController);
            }
        }
    }
}
