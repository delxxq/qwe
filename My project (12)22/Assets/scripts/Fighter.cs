using UnityEngine;
using System.Collections.Generic;

public class Fighter : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public ActionType currentAction;

    [Header("Настройки бота")]
    // Список ударов, которые бот будет делать ВСЕГДА в этом порядке
    public List<ActionType> botFixedPattern = new List<ActionType>();

    private int patternIndex = 0;

    void Awake() => health = maxHealth;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
    }

    // Сброс индекса, чтобы в новом бою бот начинал с первого удара в списке
    public void ResetPattern()
    {
        patternIndex = 0;
    }

    // Бот просто берет следующий удар из списка
    public ActionType GetBotAction(Fighter player)
    {
        if (botFixedPattern.Count == 0) return ActionType.Jab; // На случай если список пуст

        // Берем удар по текущему индексу
        ActionType action = botFixedPattern[patternIndex % botFixedPattern.Count];

        // Переходим к следующему удару
        patternIndex++;

        return action;
    }
}
