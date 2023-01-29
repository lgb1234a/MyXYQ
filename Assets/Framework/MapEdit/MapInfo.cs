using System;
using UnityEngine;

namespace Framework {
    [Serializable]
    public class MapInfo : IMapInfo {
        public static int Default_Value = 0;
        // 路径
        public static int Path_Value = -1;
        // 障碍物
        public static int Obstacle_Value = 1;
        // 遮挡
        public static int Occlusion_Value = 2;
        public float m_gridWidth;
        public float m_gridHeight;
        public int[] m_gridValues;  //一维数组，左下角为原点，从下到上，从左往右的顺序记录
        public int m_rows;
        public int m_columns;

        public MapInfo() {}

        public MapInfo(float gridWidth, float gridHeight, int[] gridValues, int rows, int columns) {
            m_gridWidth = gridWidth;
            m_gridHeight = gridHeight;
            m_gridValues = gridValues;
            m_rows = rows;
            m_columns = columns;
        }

        public void ClearPath()
        {
            int count = GetGridValues().Length;
            for(int i = 0; i < count; i++) {
                if (IsIndexPath(i)) {
                    GetGridValues()[i] = 0;
                }
            }
        }

        /// excludeCoordinate避免往回索引已经遍历过的点导致死循环
        public Vector2Int GetNextPathTo(Vector2Int coordinate, Vector2Int excludeCoordinate)
        {
            var left = new Vector2Int(coordinate.x-1, coordinate.y);
            if (IsCoordinatePath(left) && left != excludeCoordinate)
                return left;

            var right = new Vector2Int(coordinate.x+1, coordinate.y);
            if (IsCoordinatePath(right) && right != excludeCoordinate)
                return right;

            var top = new Vector2Int(coordinate.x, coordinate.y+1);
            if (IsCoordinatePath(top) && top != excludeCoordinate)
                return top;

            var bottom = new Vector2Int(coordinate.x, coordinate.y-1);
            if (IsCoordinatePath(bottom) && bottom != excludeCoordinate)
                return bottom;

            var leftTop = new Vector2Int(coordinate.x-1, coordinate.y+1);
            if (IsCoordinatePath(leftTop) && leftTop != excludeCoordinate)
                return leftTop;

            var leftBottom = new Vector2Int(coordinate.x-1, coordinate.y-1);
            if (IsCoordinatePath(leftBottom) && leftBottom != excludeCoordinate)
                return leftBottom;

            var rightTop = new Vector2Int(coordinate.x+1, coordinate.y+1);
            if (IsCoordinatePath(rightTop) && rightTop != excludeCoordinate)
                return rightTop;

            var rightBottom = new Vector2Int(coordinate.x+1, coordinate.y-1);
            if (IsCoordinatePath(rightBottom) && rightBottom != excludeCoordinate)
                return rightBottom;
            return Vector2Int.zero;
        }


        public bool CanMove(Transform character, Vector2 destination) {
            var translation = new Vector3(destination.x, destination.y, 0) - character.position;
            translation = translation.normalized * Math.Min(m_gridWidth, m_gridHeight);
            var characterCoordinate = WorldPosition2GridCoordinate(character.position);
            var nextLocation = character.position + translation;
            var nextCoordinate = WorldPosition2GridCoordinate(nextLocation);
            if (IsCoordinateObstacle(nextCoordinate)) {
                return false;
            }
            return true;
        }

        public int[] GetGridValues() {
            return m_gridValues;
        }

        public void SetGridValues(int[] v) {
            m_gridValues = v;
        }

        public float GetGridWidth() {
            return m_gridWidth;
        }

        public void SetGridWidth(float v) {
            m_gridWidth = v;
        }

        public float GetGridHeight() {
            return m_gridHeight;
        }

        public void SetGridHeight(float v) {
            m_gridHeight = v;
        }

        public int GetGridRows() {
            return m_rows;
        }

        public void SetGridRows(int v) {
            m_rows = v;
        }

        public int GetGridColumns() {
            return m_columns;
        }

        public void SetGridColumns(int v) {
            m_columns = v;
        }

        public void SetGridValue(Vector2Int coordinate, int value) {
            m_gridValues[GridCoordinate2Index(coordinate)] = value;
        }

        public int GetGridValue(Vector2Int coordinate) {
            return m_gridValues[GridCoordinate2Index(coordinate)];
        }

        public Vector2Int GridIndex2Coordinate(int index) {
            int x = index/m_rows;
            int y = index%m_columns;
            return new Vector2Int(x, y);
        }

        public Vector2 GridCoordinate2WorldPosition(Vector2Int coordinate) {
            var idx = GridCoordinate2Index(coordinate);
            return GridIndex2WorldPosition(idx);
        }

        public int GridCoordinate2Index(Vector2Int coordinate) {
            int index = coordinate.x*m_rows + coordinate.y;
            return index;
        }

        public Vector2 GridIndex2WorldPosition(int index) {
            Vector2Int coordinate = GridIndex2Coordinate(index);
            float positionX = coordinate.x*m_gridWidth + m_gridWidth*0.5f;
            float positionY = coordinate.y*m_gridHeight + m_gridHeight*0.5f;
            return new Vector2(positionX, positionY);
        }

        public Vector2Int WorldPosition2GridCoordinate(Vector2 position) {
            int tx = (int)(position.x/m_gridWidth);
            int ty = (int)(position.y/m_gridHeight);
            return new Vector2Int(tx, ty);
        }

        public bool IsCoordinatePath(Vector2Int coordinate) {
            return GetGridValue(coordinate) == Path_Value;
        }

        public bool IsCoordinateObstacle(Vector2Int coordinate) {
            return GetGridValue(coordinate) == Obstacle_Value;
        }

        public bool IsCoordinateOcclusion(Vector2Int coordinate) {
            return GetGridValue(coordinate) == Occlusion_Value;
        }

        public bool IsCoordinateDefaultValue(Vector2Int coordinate) {
            return GetGridValue(coordinate) == Default_Value; 
        }

        public bool IsIndexPath(int index) {
            return m_gridValues[index] == Path_Value;
        }

        public bool IsIndexObstacle(int index) {
            return m_gridValues[index] == Obstacle_Value;
        }

        public bool IsIndexOcclusion(int index) {
            return m_gridValues[index] == Occlusion_Value;
        }
    }
}