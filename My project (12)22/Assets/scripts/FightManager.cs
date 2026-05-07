using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    public string nextSceneName;
    [Header("Персонажи")]
    public Fighter player;
    public Fighter bot;
    [Header("UI Элементы (Тексты)")]
    public Text playerHealthText;
    public Text botHealthText;
    public Text resultText;
    public Text sequenceText;
    [Header("UI Элементы (Картинки)")]
    public Image playerActionImage;
    public Image botActionImage;
    [Header("Кнопки действий")]
    public Button jabButton;
    public Button uppercutButton;
    public Button overhandButton;
    public Button dodgeButton;
    public Button startFightButton;
    public Button clearButton;
    [Header("Спрайты Игрока")]
    public Sprite playerJabSprite;
    public Sprite playerUppercutSprite;
    public Sprite playerOverhandSprite;
    public Sprite playerDodgeSprite;
    public Sprite playerNoneSprite;
    [Header("Спрайты Бота")]
    public Sprite botJabSprite;
    public Sprite botUppercutSprite;
    public Sprite botOverhandSprite;
    public Sprite botDodgeSprite;
    public Sprite botNoneSprite;

    private List<ActionType> playerPlannedActions = new List<ActionType>();
    private bool isFighting = false;

    void Start()
    {
        UpdateHealthUI();
        resultText.text = "Спланируй серию ударов!";
        sequenceText.text = "План: ";
        if (jabButton != null) jabButton.onClick.AddListener(PlayerJab);
        if (uppercutButton != null) uppercutButton.onClick.AddListener(PlayerUppercut);
        if (overhandButton != null) overhandButton.onClick.AddListener(PlayerOverhand);
        if (dodgeButton != null) dodgeButton.onClick.AddListener(PlayerDodge);
        if (clearButton != null) clearButton.onClick.AddListener(ClearPlan);
        if (startFightButton != null)
        {
            startFightButton.onClick.AddListener(StartFight);
            startFightButton.interactable = false;
        }
        SetActionButtonsInteractable(true);
    }
    void SetActionButtonsInteractable(bool interactable)
    {
        if (jabButton != null) jabButton.interactable = interactable;
        if (uppercutButton != null) uppercutButton.interactable = interactable;
        if (overhandButton != null) overhandButton.interactable = interactable;
        if (dodgeButton != null) dodgeButton.interactable = interactable;
        if (clearButton != null) clearButton.interactable = interactable;
    }
    public void PlayerJab() => AddToPlan(ActionType.Jab);
    public void PlayerUppercut() => AddToPlan(ActionType.Uppercut);
    public void PlayerOverhand() => AddToPlan(ActionType.Overhand);
    public void PlayerDodge() => AddToPlan(ActionType.Dodge);

    private void AddToPlan(ActionType action)
    {
        if (isFighting) return;
        playerPlannedActions.Add(action);
        if (sequenceText != null)
        {
            string actionName = "";
            switch (action)
            {
                case ActionType.Jab: actionName = "Джеб"; break;
                case ActionType.Uppercut: actionName = "Апперкот"; break;
                case ActionType.Overhand: actionName = "Оверхенд"; break;
                case ActionType.Dodge: actionName = "Уворот"; break;
            }
            sequenceText.text += actionName + " > ";
        }
        if (startFightButton != null) startFightButton.interactable = true;
    }

    public void ClearPlan()
    {
        if (isFighting) return;
        playerPlannedActions.Clear();
        sequenceText.text = "План: ";
        startFightButton.interactable = false;
    }
    
public void StartFight()
    {
        if (playerPlannedActions.Count > 0 && !isFighting)
        {
            StartCoroutine(AutoFightCoroutine());
        }
    }

    IEnumerator AutoFightCoroutine()
    {
        isFighting = true;
        if (startFightButton != null) startFightButton.interactable = false;
        SetActionButtonsInteractable(false);

        bot.ResetPattern();

        foreach (ActionType plannedAction in playerPlannedActions)
        {
            if (player.health <= 0 || bot.health <= 0) break;
            player.currentAction = plannedAction;
            bot.currentAction = bot.GetBotAction(player);
            SetPlayerActionImage(player.currentAction);
            SetBotActionImage(bot.currentAction);
            yield return new WaitForSeconds(0.6f);
            string roundResult = ExecuteActions();
            resultText.text = roundResult;
            UpdateHealthUI();
            yield return new WaitForSeconds(1.2f);
        }

        if (player.health <= 0 || bot.health <= 0)
        {
            isFighting = false;

            if (player.health <= 0)
                resultText.text = "БОТ ПОБЕДИЛ!";
            else
                resultText.text = "ИГРОК ПОБЕДИЛ!";
            yield return new WaitForSeconds(2f);
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                resultText.text = "ФИНАЛ ИГРЫ!";
            }

            yield break; 
        }
    }

    string ExecuteActions()
    {
        ActionType pa = player.currentAction;
        ActionType ba = bot.currentAction;

        if (pa == ActionType.Dodge && IsAttack(ba)) return "Вы уклонились от удара бота!";
        if (ba == ActionType.Dodge && IsAttack(pa)) return "Бот уклонился от вашего удара!";

        if (IsAttack(pa) && IsAttack(ba))
        {
            int pDmg = GetDamage(pa);
            int bDmg = GetDamage(ba);
            bot.TakeDamage(pDmg);
            player.TakeDamage(bDmg);
            return $"Обмен! Вы: {pDmg} | Бот: {bDmg}";
        }
        if (IsAttack(pa))
        {
            int dmg = GetDamage(pa);
            bot.TakeDamage(dmg);
            return $"Вы нанесли {dmg} урона!";
        }
        if (IsAttack(ba))
        {
            int dmg = GetDamage(ba);
            player.TakeDamage(dmg);
            return $"Бот нанёс {dmg} урона!";
        }
        return "Ничего не произошло...";
    }

    bool IsAttack(ActionType a) => a == ActionType.Jab || a == ActionType.Uppercut || a == ActionType.Overhand;

    int GetDamage(ActionType a)
    {
        switch (a)
        {
            case ActionType.Jab: return 10;
            case ActionType.Uppercut: return 15;
            case ActionType.Overhand: return 20;
            default: return 0;
        }
    }

    void UpdateHealthUI()
    {
        if (playerHealthText != null) playerHealthText.text = $"Игрок: {player.health}";
        if (botHealthText != null) botHealthText.text = $"Бот: {bot.health}";
    }
    void SetPlayerActionImage(ActionType action)
    {
        if (playerActionImage == null) return;

        switch (action)
        {
            case ActionType.Jab: playerActionImage.sprite = playerJabSprite; break;
            case ActionType.Uppercut:
                playerActionImage.sprite = playerUppercutSprite; break;
                


case ActionType.Overhand: playerActionImage.sprite = playerOverhandSprite; break;
            case ActionType.Dodge: playerActionImage.sprite = playerDodgeSprite; break;
            default: playerActionImage.sprite = playerNoneSprite; break;
        }
    }

    void SetBotActionImage(ActionType action)
    {
        if (botActionImage == null) return;

        switch (action)
        {
            case ActionType.Jab: botActionImage.sprite = botJabSprite; break;
            case ActionType.Uppercut: botActionImage.sprite = botUppercutSprite; break;
            case ActionType.Overhand: botActionImage.sprite = botOverhandSprite; break;
            case ActionType.Dodge: botActionImage.sprite = botDodgeSprite; break;
            default: botActionImage.sprite = botNoneSprite; break;
        }
    }
}