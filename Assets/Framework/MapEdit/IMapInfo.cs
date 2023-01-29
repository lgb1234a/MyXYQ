using UnityEngine;

namespace Framework {
    public interface IMapInfo {
        public bool IsCoordinateObstacle(Vector2Int coordinate);

        public bool IsCoordinateOcclusion(Vector2Int coordinate);

        public bool IsCoordinateDefaultValue(Vector2Int coordinate);

        public bool IsIndexPath(int index);

        public void SetGridValue(Vector2Int coordinate, int value);

        public int GetGridValue(Vector2Int coordinate);

        public Vector2Int GridIndex2Coordinate(int index);

        public Vector2Int WorldPosition2GridCoordinate(Vector2 position);

        public Vector2 GridCoordinate2WorldPosition(Vector2Int coordinate);

        public int GridCoordinate2Index(Vector2Int coordinate);

        public Vector2 GridIndex2WorldPosition(int index);

        public void ClearPath();

        public Vector2Int GetNextPathTo(Vector2Int coordinate, Vector2Int excludeCoordinate);

        public bool CanMove(Transform character, Vector2 destination);

        public int[] GetGridValues();

        public void SetGridValues(int[] v);

        public float GetGridWidth();

        public void SetGridWidth(float v);

        public float GetGridHeight();

        public void SetGridHeight(float v);

        public int GetGridRows();

        public void SetGridRows(int v);

        public int GetGridColumns();

        public void SetGridColumns(int v);
    }
}