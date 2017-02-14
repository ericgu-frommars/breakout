using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine;

namespace Breakout.DataLoader
{
    public struct TileData
    {
        public float x;
        public float y;
        public int   type;
    }

    public class LevelConfig
    {
        public List<TileData> m_tileDataList = new List<TileData>();
    }

    /// <summary>
    /// Load level xml data
    /// </summary>
    public class XmlDataLoader
    {
        private Dictionary<string, LevelConfig> m_levelConfigDict = new Dictionary<string, LevelConfig>();

        public XmlDataLoader()
        {

        }

        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();

            TextAsset levelConfigXML = (TextAsset)Resources.Load("Xml/Level", typeof(TextAsset));
            xmlDoc.LoadXml(levelConfigXML.text);

            XmlNodeList levelConfigNodeList = xmlDoc.SelectSingleNode("resources").ChildNodes;
		    foreach (XmlElement levelConfigNode in levelConfigNodeList) 
		    {
                LevelConfig levelConfig = new LevelConfig();

                //Level Name
			    string levelName = levelConfigNode.GetAttribute("Name").ToString();
			    
                XmlNodeList tileDataNodeList = levelConfigNode.SelectNodes("TileData");
                for (int index = 0; index < tileDataNodeList.Count; index++)
                {
                    XmlNode tileDataNode = tileDataNodeList.Item(index);
                    XmlNode propertyNode;
                     
                    propertyNode = tileDataNode.SelectSingleNode("X");
                    string posXInfo = propertyNode.InnerText;

                    propertyNode = tileDataNode.SelectSingleNode("Y");
                    string posYInfo = propertyNode.InnerText;

                    propertyNode = tileDataNode.SelectSingleNode("Type");
                    string typeInfo = propertyNode.InnerText;

                    //
                    TileData tileData = new TileData();
                    tileData.x = float.Parse(posXInfo);
                    tileData.y = float.Parse(posYInfo);
                    tileData.type = int.Parse(typeInfo);
                    levelConfig.m_tileDataList.Add(tileData);
                }

                m_levelConfigDict.Add(levelName, levelConfig);
		    }
        }

        public LevelConfig GetLevelConfig(string levelName)
        {
            if (m_levelConfigDict.ContainsKey(levelName))
            {
                return m_levelConfigDict[levelName];
            }

            return null;
        }
    }
}
