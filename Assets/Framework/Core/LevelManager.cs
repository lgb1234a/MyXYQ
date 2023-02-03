using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class LevelManager
    {
        private LevelInfo m_currentLevel;
        private IMapInfo m_mapInfo;
        private AStar m_aStar;
        private IEnumerator m_aStarProcess;

        private static LevelManager instance;

        public static LevelManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new LevelManager();
                return instance;
            }
        }

        private LevelManager() {
            m_currentLevel = new LevelInfo();
            m_aStar = new AStar();

            var mapName = GetCurrentLevelMapName();
            string path = Environment.GetMapJsonDataPath(mapName);
            m_mapInfo = TextAssetRWAblity.ImportFromFile<MapInfo>(path);

            var mapSize = new Vector2Int(m_mapInfo.GetGridRows(), m_mapInfo.GetGridColumns());
            m_aStar.Init(m_mapInfo, mapSize, EvaluationFunctionType.Manhattan);
        }

        private string GetCurrentLevelMapName()
        {
            return m_currentLevel.m_mapName;
        }

        public IMapInfo GetMapInfo()
        {
            return m_mapInfo;
        }

        public void ClearMapInfo()
        {
            m_mapInfo = null;
        }

        public IEnumerable<Vector2> Navigate(Vector2 playerLocation, Vector2 destinationLocation) {
            var playerCoordinate = m_mapInfo.WorldPosition2GridCoordinate(playerLocation);
            var destinationCoordinate = m_mapInfo.WorldPosition2GridCoordinate(destinationLocation);
            return Navigate(playerCoordinate, destinationCoordinate);
        } 

        public IEnumerable<Vector2> Navigate(Vector2Int playerCoordinate, Vector2Int destinationCoordinate) {
            m_mapInfo.ClearPath();
            m_aStarProcess = m_aStar.Start(playerCoordinate, destinationCoordinate);
            while(m_aStarProcess.MoveNext())
                ;
            
            var nextCoordinate = playerCoordinate;
            var lastCoordinate = playerCoordinate;
            while(m_mapInfo.GetNextPathTo(nextCoordinate, lastCoordinate) != Vector2Int.zero) {
                var tempCoordinate = lastCoordinate;
                lastCoordinate = nextCoordinate;
                nextCoordinate = m_mapInfo.GetNextPathTo(nextCoordinate, tempCoordinate);
                yield return m_mapInfo.GridCoordinate2WorldPosition(nextCoordinate);
            }
        }

        public bool CanMove(Transform character, Vector2 destination) {
            return m_mapInfo.CanMove(character, destination);
        }

        public Vector2 GetGridSize() {
            return new Vector2(m_mapInfo.GetGridWidth(), m_mapInfo.GetGridHeight());
        }

        public bool IsCoordinateObstacle(Vector2 destination) {
            var destCoordinate = m_mapInfo.WorldPosition2GridCoordinate(destination);
            return m_mapInfo.IsCoordinateObstacle(destCoordinate);
        }
    }
}
