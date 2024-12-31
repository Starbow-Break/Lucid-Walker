using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public static class PoissonDiskSampling
{   
    // 포인트 생성
    public static List<Vector2> GeneratePoints(float radius, Vector2 regionSize, int numSampleTry = 10) {
        float cellSize = radius / Mathf.Sqrt(2); // 셀 크기
        int[,] grid = new int[Mathf.CeilToInt(regionSize.x / cellSize), Mathf.CeilToInt(regionSize.y / cellSize)]; // 그리드

        List<Vector2> points = new List<Vector2>(); // 최종 생성된 포인트들
        List<Vector2> spawnPoints = new List<Vector2> { regionSize / 2 }; // 포인트 생성에 사용될 점들, 영역 중점에서 생성 시작

        // 생성 포인트들이 전부 소멸할 때까지 포인트 생성 
        while(spawnPoints.Count > 0) {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            bool generateSuccess = false;

            for(int i = 0; i < numSampleTry; i++) {
                float angle = Random.Range(0, 2 * Mathf.PI);
                Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
                float dist = Random.Range(radius, 2 * radius);
                Vector2 newPoint = spawnPoints[spawnIndex] + dist * dir;

                if(IsValid(newPoint, radius, regionSize, cellSize, grid, points)) {
                    points.Add(newPoint);
                    spawnPoints.Add(newPoint);
                    grid[Mathf.FloorToInt(newPoint.x / cellSize), Mathf.FloorToInt(newPoint.y / cellSize)] = points.Count;
                    generateSuccess = true;
                    break;
                }
            }

            if(!generateSuccess) {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    // 해당 포인트가 유효한지 확인
    public static bool IsValid(Vector2 point, float radius, Vector2 regionSize, float cellSize, int[,] grid, List<Vector2> generatedPoints) {
        if(point.x >= 0 && point.x <= regionSize.x && point.y >= 0 && point.y <= regionSize.y) {
            Vector2Int cell = new(Mathf.FloorToInt(point.x / cellSize), Mathf.FloorToInt(point.y / cellSize));
            int minX = Mathf.Max(0, cell.x - 2);
            int maxX = Mathf.Min(cell.x + 2, grid.GetLength(0) - 1);
            int minY = Mathf.Max(0, cell.y - 2);
            int maxY = Mathf.Min(cell.y + 2, grid.GetLength(1) - 1);

            for(int x = minX; x <= maxX; x++) {
                for(int y = minY; y <= maxY; y++) {
                    if(grid[x, y] > 0) {
                        float dist = (point - generatedPoints[grid[x, y] - 1]).magnitude;
                        if(dist < radius) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        return false;
    }
}
