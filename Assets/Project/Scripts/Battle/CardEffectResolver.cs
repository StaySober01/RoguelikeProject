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
                    battleManager.DealAttackDamage(battleManager.enemyUnit, effect.amount);
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

            case "toxic_burst":
                int poison = battleManager.statusEffectController.GetStack(
                    battleManager.enemyUnit, StatusEffectType.Poison);
                battleManager.DealAttackDamage(battleManager.enemyUnit, poison * 2);
                Debug.Log("Toxic Burst: Enemy takes twice the amount of poison.");
                break;

            case "toxic_stacking":
                bool wasPoisoned = battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit, StatusEffectType.Poison);

                battleManager.statusEffectController.ApplyPoison(
                    battleManager.enemyUnit, 1);

                if (wasPoisoned)
                {
                    battleManager.statusEffectController.ApplyPoison(
                        battleManager.enemyUnit, 2);
                }

                Debug.Log("Toxic Stacking: Apply 1 Poison. If target was already poisoned, apply 2 additional Poison.");
                break;

            case "flame_accelerate":
                int burn = battleManager.statusEffectController.GetStack(
                    battleManager.enemyUnit, StatusEffectType.Burn);
                battleManager.DrawCards(burn);
                Debug.Log("Flame Eccelerate: Draw cards equal to the target's Burn stacks.");
                break;

            case "vulnerable":
                battleManager.statusEffectController.ApplyVulnerable(
                    battleManager.enemyUnit,
                    2
                );
                Debug.Log("Vulnerable: Apply 2 Vulnerable.");
                break;

            case "spot_weakness":
                bool wasVulnerable = battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit, StatusEffectType.Vulnerable);

                battleManager.DealAttackDamage(battleManager.enemyUnit, 7);
                Debug.Log("Spot Weakness: Deal 7 damage.");

                if (wasVulnerable)
                {
                    battleManager.DealAttackDamage(battleManager.enemyUnit, 7);
                    Debug.Log("Spot Weakness: Target is Vulnerable, trigger once more.");
                }
                break;

            case "taunt":
                battleManager.playerUnit.AddBlock(3);
                battleManager.statusEffectController.ApplyVulnerable(
                    battleManager.enemyUnit,
                    2
                );
                Debug.Log("Taunt: Gain 3 Block and apply 2 Vulnerable.");
                break;

            case "poisonic_fury":
                wasPoisoned = battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit,
                    StatusEffectType.Poison
                );

                battleManager.statusEffectController.ApplyVulnerable(
                    battleManager.enemyUnit,
                    2
                );

                if (wasPoisoned)
                {
                    battleManager.DealAttackDamage(battleManager.enemyUnit, 5);
                    Debug.Log("Poisonic Fury: Target was Poisoned, deal 5 damage.");
                }
                else
                {
                    Debug.Log("Poisonic Fury: Apply 2 Vulnerable.");
                }
                break;

            case "pressure":
                wasVulnerable = battleManager.statusEffectController.HasStatus(
                    battleManager.enemyUnit,
                    StatusEffectType.Vulnerable
                );

                battleManager.DealAttackDamage(battleManager.enemyUnit, 10);
                Debug.Log("Pressure: Deal 10 damage.");

                if (wasVulnerable)
                {
                    battleManager.GainEnergy(1);
                    Debug.Log("Pressure: Target was Vulnerable, gain 1 Energy.");
                }
                break;
        }
    }
}