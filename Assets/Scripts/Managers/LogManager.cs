using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class LogManager : Singleton<LogManager>
    {
        int _id = 0;
        public List<LogCluster> logClusterList = new();
        public List<TextCluster> textClusters = new();

        public int GetNextID() => _id++;

        #region 생성자
        LogManager() { }
        #endregion

        /// <summary> 새로하기 선택 시 육성 데이터 초기화 </summary>
        public void ResetLogData()
        {
            _id = 0;
            logClusterList = new();
            textClusters = new();
        }
        public TextCluster GetNewClusterGroup(string title)
        {
            TextCluster cluster = new TextCluster(title);
            textClusters.Add(cluster);
            return cluster;
        }
        /// <summary> 마지막 로그 그룹 가져오기 </summary>
        public TextCluster GetLastClusterGroup()
        {
            return textClusters[^1];
        }

    }
}
