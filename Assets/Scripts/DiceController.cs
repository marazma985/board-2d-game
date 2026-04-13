using UnityEngine;
using TMPro; // Нужно для работы с TextMeshPro

public class DiceController : MonoBehaviour
{
    [Header("Настройки UI")]
    // В веб-терминах: это как получить элемент по ID, но мы передаем его через Инспектор
    [SerializeField] private TextMeshProUGUI resultText;

    // Время, через которое результат исчезнет (в секундах)
    [SerializeField] private float hideDelay = 2.0f;

    // Метод, который будет вешаться на кнопку
    // В веб-разработке это как обработчик onclick
    public void RollDice()
    {
        // 1. Показываем текст (включаем объект)
        resultText.gameObject.SetActive(true);

        // 2. Генерируем число от 1 до 6 (верхняя граница в Random.Range исключительная)
        int diceValue = Random.Range(1, 7);

        // 3. Выводим на экран
        resultText.text = diceValue.ToString();

        // 4. Планируем скрытие через время
        // Invoke - это аналог setTimeout в JS
        Invoke(nameof(HideResult), hideDelay);
    }

    private void HideResult()
    {
        resultText.gameObject.SetActive(false);
    }
}