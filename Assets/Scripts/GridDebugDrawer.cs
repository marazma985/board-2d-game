using UnityEngine;

/// <summary>
/// Скрипт для отрисовки сетки на фоне для визуальной отладки
/// </summary>
public class GridDebugDrawer : MonoBehaviour
{
    [Header("Настройки сетки")]
    public Grid gameGrid;
    
    [Header("Настройки отрисовки")]
    public Color gridColor = Color.gray;
    public float lineWidth = 0.02f;
    public int gridWidth = 20;  // Количество клеток по ширине
    public int gridHeight = 20; // Количество клеток по высоте
    
    private Material lineMaterial;
    
    void Start()
    {
        if (gameGrid == null)
            gameGrid = FindObjectOfType<Grid>();
            
        // Создаем материал для линий
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Legacy Shaders/Diffuse");
            
        lineMaterial = new Material(shader);
        lineMaterial.color = gridColor;
    }
    
    void OnPostRender()
    {
        if (gameGrid == null || lineMaterial == null)
            return;
            
        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(gridColor);
        
        Vector3Int startCell = new Vector3Int(-gridWidth / 2, -gridHeight / 2, 0);
        Vector3Int endCell = new Vector3Int(gridWidth / 2, gridHeight / 2, 0);
        
        // Вертикальные линии
        for (int x = startCell.x; x <= endCell.x; x++)
        {
            Vector3 top = GetCellCornerWorld(new Vector3Int(x, endCell.y, 0), true);
            Vector3 bottom = GetCellCornerWorld(new Vector3Int(x, startCell.y, 0), false);
            GL.Vertex(top);
            GL.Vertex(bottom);
        }
        
        // Горизонтальные линии
        for (int y = startCell.y; y <= endCell.y; y++)
        {
            Vector3 left = GetCellCornerWorld(new Vector3Int(startCell.x, y, 0), false);
            Vector3 right = GetCellCornerWorld(new Vector3Int(endCell.x, y, 0), true);
            GL.Vertex(left);
            GL.Vertex(right);
        }
        
        GL.End();
        GL.PopMatrix();
    }
    
    Vector3 GetCellCornerWorld(Vector3Int cell, bool maxCorner)
    {
        Vector3 worldPos = gameGrid.CellToWorld(cell);
        Vector3 cellSize = gameGrid.cellSize;
        
        if (maxCorner)
            return worldPos + new Vector3(cellSize.x, cellSize.y, 0);
        else
            return worldPos;
    }
    
    void OnDrawGizmos()
    {
        if (gameGrid == null)
            gameGrid = FindObjectOfType<Grid>();
            
        if (gameGrid == null)
            return;
            
        Gizmos.color = gridColor;
        
        Vector3Int startCell = new Vector3Int(-gridWidth / 2, -gridHeight / 2, 0);
        Vector3Int endCell = new Vector3Int(gridWidth / 2, gridHeight / 2, 0);
        
        // Рисуем вертикальные линии
        for (int x = startCell.x; x <= endCell.x; x++)
        {
            Vector3 top = GetCellCornerWorld(new Vector3Int(x, endCell.y, 0), true);
            Vector3 bottom = GetCellCornerWorld(new Vector3Int(x, startCell.y, 0), false);
            Gizmos.DrawLine(top, bottom);
        }
        
        // Рисуем горизонтальные линии
        for (int y = startCell.y; y <= endCell.y; y++)
        {
            Vector3 left = GetCellCornerWorld(new Vector3Int(startCell.x, y, 0), false);
            Vector3 right = GetCellCornerWorld(new Vector3Int(endCell.x, y, 0), true);
            Gizmos.DrawLine(left, right);
        }
    }
}
