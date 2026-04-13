using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoverNew : MonoBehaviour
{
    public Grid gameGrid;
    public float moveSpeed = 10f; // Скорость перемещения между клетками

    private Vector3Int targetCell; // Целевая ячейка (координаты сетки)
    private bool isMoving = false;
    private Vector3 currentStepTarget; // Куда движемся прямо сейчас (соседняя клетка)

    void Start()
    {
        if (gameGrid == null) gameGrid = FindObjectOfType<Grid>();
        SnapToGridInstantly();
    }

    // Твой код для старта на сетке
    void SnapToGridInstantly()
    {
        if (gameGrid == null) return;
        Vector3Int cell = gameGrid.WorldToCell(transform.position);
        transform.position = GetCellCenterWorld(cell);
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
            isMoving = true;

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

                // Если мы еще не в финальной точке, планируем следующий шаг
                Vector3Int currentCell = gameGrid.WorldToCell(transform.position);
                if (currentCell != targetCell)
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

    // Логика выбора следующего шага (4 направления - только ортогонально)
    void PlanNextStep()
    {
        Vector3Int currentCell = gameGrid.WorldToCell(transform.position);

        int diffX = targetCell.x - currentCell.x;
        int diffY = targetCell.y - currentCell.y;

        // Двигаемся сначала по оси с наибольшей разницей (ортогональное движение)
        // Это предотвращает диагональное движение и зацикливание
        int stepX = 0;
        int stepY = 0;

        if (Mathf.Abs(diffX) >= Mathf.Abs(diffY))
        {
            // Двигаемся по X
            if (diffX != 0)
                stepX = (int)Mathf.Sign(diffX);
            else if (diffY != 0)
                stepY = (int)Mathf.Sign(diffY);
        }
        else
        {
            // Двигаемся по Y
            if (diffY != 0)
                stepY = (int)Mathf.Sign(diffY);
            else if (diffX != 0)
                stepX = (int)Mathf.Sign(diffX);
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