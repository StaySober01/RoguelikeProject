using System;
using UnityEngine;
using UnityEngine.XR;

public class CardEffectResolver
{
    public void Resolve(BattleManager battleManager, CardInstance card)
    {
        foreach (var effect in card.Effects)
        {
            switch (effect.effectType)
            {
                case CardEffectType.DealDamage:
                    battleManager.enemyUnit.TakeDamage(effect.amount);
                    break;

                case CardEffectType.GainBlock:
                    battleManager.playerUnit.AddBlock(effect.amount);
                    break;

                case CardEffectType.ApplyPoison:
                    battleManager.statusEffectController.ApplyPoison(
                        battleManager.enemyUnit, effect.amount);
                    break;

                case CardEffectType.ApplyBurn:
                    battleManager.statusEffectController.ApplyBurn(
                        battleManager.enemyUnit, effect.amount);
                    break;

                case CardEffectType.DrawCards:
                    battleManager.DrawCards(effect.amount);
                    break;
            }
        }

        ResolveSpecialLogic(battleManager, card);
    }

    private void ResolveSpecialLogic(BattleManager battleManager, CardInstance card)
    {
        switch (card.CardId)
        {
            case "venom_guard":
                if (battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit, StatusEffectType.Poison))
                {
                    battleManager.playerUnit.AddBlock(4);
                    Debug.Log($"{card.CardName}: Target was Poisoned, gain 4 Block.");
                }
                break;

            case "toxic_ignition":
                if (battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit, StatusEffectType.Poison))
                {
                    battleManager.AddRandomCardWithTagFromDrawPileToHand(CardTag.Burn);
                    Debug.Log($"{card.CardName}: Target was Poisoned, fetched a Burn-related card.");
                }
                break;

            case "afterflare":
                bool exploded = battleManager.statusEffectController.ApplyBurn(
                    battleManager.enemyUnit, 1);

                if (exploded)
                {
                    battleManager.statusEffectController.ApplyPoison(
                        battleManager.enemyUnit, 1);
                    Debug.Log("Afterflare: Explosion occurred, apply 1 Poison.");
                }
                break;

            case "empty_arsenal":
                bool hasAttackCard = battleManager.hand.Exists(
                    c => c.Category == CardCategory.Attack);

                if (!hasAttackCard)
                {
                    battleManager.DrawCards(2);
                    Debug.Log("Empty Arsenal: No Attack cards in hand, draw 2.");
                }
                else
                {
                    Debug.Log("Empty Arsenal: Attack card exists in hand.");
                }
                break;

            case "heat_charge":
                if (battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit, StatusEffectType.Burn))
                {
                    battleManager.GainEnergy(1);
                    Debug.Log("Heat Charge: Enemy has Burn, gain 1 Energy.");
                }
                else
                {
                    Debug.Log("Heat Charge: Enemy does not have Burn.");
                }
                break;

            case "overclocked_flames":
                battleManager.EnableDoubleExplosionDamageThisTurn();
                Debug.Log("Overclocked Flames: Explosion damage is doubled this turn.");
                break;
        }
    }
}