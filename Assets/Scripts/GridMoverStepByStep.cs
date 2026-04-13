using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMoverStepByStep : MonoBehaviour
{
    public Grid gameGrid;
    public float moveSpeed = 10f;

    // Логическая позиция (где мы "думаем" что находимся)
    private Vector3Int currentCell;
    // Целевая позиция (куда кликнули)
    private Vector3Int targetCell;

    // Физическая цель для текущего шага (координаты в мире)
    private Vector3 stepTargetWorldPos;

    private bool isMoving = false;

    void Start()
    {
        if (gameGrid == null) gameGrid = FindObjectOfType<Grid>();

        // Инициализация: ставим куб на сетку и запоминаем клетку
        SnapToGridInstantly();
    }

    void SnapToGridInstantly()
    {
        if (gameGrid == null) return;
        // Определяем клетку по текущей позиции
        currentCell = gameGrid.WorldToCell(transform.position);
        // Телепортируем в центр этой клетки
        transform.position = GetCellCenterWorld(currentCell);
    }

    void Update()
    {
        // 1. Обработка клика
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            targetCell = gameGrid.WorldToCell(mousePos);

            // Игнорируем клик, если кликнули в ту же клетку, где стоим
            if (targetCell == currentCell) return;

            isMoving = true;
            PlanNextStep();
        }

        // 2. Движение
        if (isMoving)
        {
            // Двигаем трансформ к цели шага
            transform.position = Vector3.MoveTowards(transform.position, stepTargetWorldPos, moveSpeed * Time.deltaTime);

            // 3. Проверка достижения шага (с запасом погрешности)
            if (Vector3.Distance(transform.position, stepTargetWorldPos) < 0.05f)
            {
                // "Примагничиваем" к идеальной позиции, чтобы убрать погрешность float
                transform.position = stepTargetWorldPos;

                // Обновляем логическую позицию (теперь мы официально в новой клетке)
                currentCell = GetCellFromWorldPos(stepTargetWorldPos);

                // Если дошли до глобальной цели — стоп
                if (currentCell == targetCell)
                {
                    isMoving = false;
                }
                else
                {
                    // Иначе планируем следующий шаг
                    PlanNextStep();
                }
            }
        }
    }

    void PlanNextStep()
    {
        // Считаем разницу между ЦЕЛЬЮ и ТЕКУЩЕЙ ЛОГИЧЕСКОЙ позицией
        int diffX = targetCell.x - currentCell.x;
        int diffY = targetCell.y - currentCell.y;

        // ИСПРАВЛЕНО: Явное приведение float к int
        int stepX = (int)Mathf.Sign(diffX);
        int stepY = (int)Mathf.Sign(diffY);

        // Вычисляем следующую клетку
        Vector3Int nextCell = new Vector3Int(currentCell.x + stepX, currentCell.y + stepY, 0);

        // Запоминаем куда бежать в этом шаге
        stepTargetWorldPos = GetCellCenterWorld(nextCell);
    }

    // Вспомогательная функция для получения центра клетки
    Vector3 GetCellCenterWorld(Vector3Int cell)
    {
        // CellToWorld возвращает нижний левый угол, добавляем половину размера для центра
        Vector3 cellSize = gameGrid.cellSize;
        Vector3 corner = gameGrid.CellToWorld(cell);
        return corner + new Vector3(cellSize.x / 2, cellSize.y / 2, 0);
    }

    // Надежное получение клетки из координат мира
    Vector3Int GetCellFromWorldPos(Vector3 worldPos)
    {
        return gameGrid.WorldToCell(worldPos);
    }
}