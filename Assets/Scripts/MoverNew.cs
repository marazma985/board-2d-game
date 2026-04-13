using System;
using UnityEngine;

public class MoverNew : MonoBehaviour
{
    public Grid gameGrid;
    public float moveSpeed = 10f; // Скорость перемещения между клетками

    private Vector3Int targetCell; // Целевая ячейка (координаты сетки)
    private bool isMoving = false;
    private Vector3 currentStepTarget; // Куда движемся прямо сейчас (соседняя клетка)
    private Vector3Int lastKnownCell; // Последняя известная позиция в клетках

    void Start()
    {
        if (gameGrid == null) gameGrid = FindObjectOfType<Grid>();
        SnapToGridInstantly();
        lastKnownCell = gameGrid.WorldToCell(transform.position);
    }

    // Твой код для старта на сетке
    void SnapToGridInstantly()
    {
        if (gameGrid == null) return;
        Vector3Int cell = gameGrid.WorldToCell(transform.position);
        transform.position = GetCellCenterWorld(cell);
        lastKnownCell = cell;
    }

    void Update()
    {
        // 1. Клик мыши - установка ГЛОБАЛЬНОЙ цели
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Запоминаем целевую клетку
            targetCell = gameGrid.WorldToCell(mousePos);
            
            // Получаем текущую клетку (округленную)
            Vector3Int currentCell = GetRoundedCurrentCell();
            
            // Если уже в целевой клетке - ничего не делаем
            if (currentCell == targetCell)
            {
                isMoving = false;
                return;
            }
            
            isMoving = true;
            lastKnownCell = currentCell;

            // Сразу планируем первый шаг
            PlanNextStep();
        }

        // 2. Движение к ТЕКУЩЕМУ шагу (соседней клетке)
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentStepTarget, moveSpeed * Time.deltaTime);

            // Если дошли до текущей промежуточной точки
            if (Vector3.Distance(transform.position, currentStepTarget) < 0.05f)
            {
                transform.position = currentStepTarget; // Фиксация

                // Обновляем известную позицию
                lastKnownCell = gameGrid.WorldToCell(transform.position);

                // Если мы еще не в финальной точке, планируем следующий шаг
                if (lastKnownCell != targetCell)
                {
                    PlanNextStep();
                }
                else
                {
                    isMoving = false; // Пришли в финал
                }
            }
        }
    }

    // Получаем текущую клетку с правильным округлением
    Vector3Int GetRoundedCurrentCell()
    {
        Vector3 worldPos = transform.position;
        Vector3 localPos = gameGrid.transform.InverseTransformPoint(worldPos);
        
        // Округляем до ближайшей клетки
        int x = Mathf.RoundToInt(localPos.x / gameGrid.cellSize.x);
        int y = Mathf.RoundToInt(localPos.y / gameGrid.cellSize.y);
        int z = Mathf.RoundToInt(localPos.z / gameGrid.cellSize.z);
        
        return new Vector3Int(x, y, z);
    }

    // Логика выбора следующего шага (4 направления - только ортогонально)
    void PlanNextStep()
    {
        // Используем lastKnownCell вместо преобразования позиции
        Vector3Int currentCell = lastKnownCell;

        int diffX = targetCell.x - currentCell.x;
        int diffY = targetCell.y - currentCell.y;

        // Двигаемся строго по одной оси за раз
        // Сначала по X, потом по Y (или наоборот, но всегда по одной оси)
        int stepX = 0;
        int stepY = 0;

        if (diffX != 0)
        {
            stepX = (int)Mathf.Sign(diffX);
        }
        else if (diffY != 0)
        {
            stepY = (int)Mathf.Sign(diffY);
        }

        // Вычисляем координаты следующей клетки
        Vector3Int nextCell = new Vector3Int(currentCell.x + stepX, currentCell.y + stepY, 0);

        currentStepTarget = GetCellCenterWorld(nextCell);
    }

    // Хелпер для получения центра клетки в мире
    Vector3 GetCellCenterWorld(Vector3Int cell)
    {
        Vector3 centerOffset = new Vector3(gameGrid.cellSize.x, gameGrid.cellSize.y, 0) / 2;
        return gameGrid.CellToWorld(cell) + centerOffset;
    }
}