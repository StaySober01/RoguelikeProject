using System.Collections.Generic;

public static class CardFactory
{
    public static CardInstance CreateAttack()
    {
        return new CardInstance(
            new CardData(
                "attack_basic",
                "Attack",
                CardCategory.Attack,
                1,
                "Deal 6 damage.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 6)
                },
                new List<CardTag>
                {
                    CardTag.Damage
                }));
    }

    public static CardInstance CreateDefend()
    {
        return new CardInstance(
            new CardData(
                "defend_basic",
                "Defend",
                CardCategory.Skill,
                1,
                "Gain 5 Block.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.GainBlock, 5)
                },
                new List<CardTag>
                {
                    CardTag.Block
                }));
    }

    public static CardInstance CreateHeavyAttack()
    {
        return new CardInstance(
            new CardData(
                "heavy_attack",
                "Heavy Attack",
                CardCategory.Attack,
                2,
                "Deal 10 damage.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 10)
                },
                new List<CardTag>
                {
                    CardTag.Damage
                }));
    }

    public static CardInstance CreatePoison()
    {
        return new CardInstance(
            new CardData(
                "poison_basic",
                "Poison",
                CardCategory.Status,
                1,
                "Apply 2 Poison.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.ApplyPoison, 2)
                },
                new List<CardTag>
                {
                    CardTag.Poison
                }));
    }

    public static CardInstance CreateBurn()
    {
        return new CardInstance(
            new CardData(
                "burn_basic",
                "Burn",
                CardCategory.Status,
                1,
                "Apply 1 Burn.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.ApplyBurn, 1)
                },
                new List<CardTag>
                {
                    CardTag.Burn
                }));
    }

    public static CardInstance CreateQuickStrike()
    {
        return new CardInstance(
            new CardData(
                "quick_strike",
                "Quick Strike",
                CardCategory.Attack,
                0,
                "Deal 2 damage. Draw 1 card.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 2),
                    new CardEffectData(CardEffectType.DrawCards, 1)
                },
                new List<CardTag>
                {
                    CardTag.Damage,
                    CardTag.Draw
                }));
    }

    public static CardInstance CreateToxicStrike()
    {
        return new CardInstance(
            new CardData(
                "toxic_strike",
                "Toxic Strike",
                CardCategory.Attack,
                1,
                "Deal 3 damage. Apply 1 Poison.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 3),
                    new CardEffectData(CardEffectType.ApplyPoison, 1)
                },
                new List<CardTag>
                {
                    CardTag.Damage,
                    CardTag.Poison
                }));
    }

    public static CardInstance CreateVenomGuard()
    {
        return new CardInstance(
            new CardData(
                "venom_guard",
                "Venom Guard",
                CardCategory.Attack,
                1,
                "Deal 3 damage. If target is Poisoned, gain 4 Block.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 3)
                },
                new List<CardTag>
                {
                    CardTag.Damage,
                    CardTag.Block
                }));
    }

    public static CardInstance CreateToxicIgnition()
    {
        return new CardInstance(
            new CardData(
                "toxic_ignition",
                "Toxic Ignition",
                CardCategory.Skill,
                1,
                "If target is Poisoned, add 1 random Burn card from draw pile to your hand.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Poison,
                CardTag.Burn
                }));
    }

    public static CardInstance CreateAfterflare()
    {
        return new CardInstance(
            new CardData(
                "afterflare",
                "Afterflare",
                CardCategory.Status,
                1,
                "Apply 1 Burn. If an explosion occurred, apply 1 Poison.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Burn,
                CardTag.Poison
                }));
    }

    public static CardInstance CreateEmptyArsenal()
    {
        return new CardInstance(
            new CardData(
                "empty_arsenal",
                "Empty Arsenal",
                CardCategory.Skill,
                0,
                "If you have no Attack cards in hand, draw 2 cards.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Draw
                }));
    }

    public static CardInstance CreateToxicEmber()
    {
        return new CardInstance(
            new CardData(
                "toxic_ember",
                "Toxic Ember",
                CardCategory.Status,
                2,
                "Apply 1 Poison. Apply 1 Burn.",
                new List<CardEffectData>
                {
                new CardEffectData(CardEffectType.ApplyPoison, 1),
                new CardEffectData(CardEffectType.ApplyBurn, 1)
                },
                new List<CardTag>
                {
                CardTag.Poison,
                CardTag.Burn
                }));
    }

    public static CardInstance CreateHeatCharge()
    {
        return new CardInstance(
            new CardData(
                "heat_charge",
                "Heat Charge",
                CardCategory.Skill,
                0,
                "If the enemy has Burn, gain 1 Energy.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Burn
                }));
    }

    public static CardInstance CreateOverclockedFlames()
    {
        return new CardInstance(
            new CardData(
                "overclocked_flames",
                "Overclocked Flames",
                CardCategory.Skill,
                0,
                "Explosion damage is doubled this turn.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Burn
                }));
    }

    public static CardInstance CreateToxicBurst()
    {
        return new CardInstance(
            new CardData(
                "toxic_burst",
                "Toxic Burst",
                CardCategory.Attack,
                2,
                "If target is poisoned, deal twice the amount of poison.",
                new List<CardEffectData>(),
                new List<CardTag> {
                    CardTag.Poison,
                    CardTag.Damage
                }));
    }

    public static CardInstance CreateToxicStacking()
    {
        return new CardInstance(
            new CardData(
                "toxic_stacking",
                "Toxic Stacking",
                CardCategory.Skill,
                2,
                "Apply 1 Poison. If target is already poisoned, apply 2 additional Poison.",
                new List<CardEffectData>(),
                new List<CardTag> {
                CardTag.Poison
                }));
    }

    public static CardInstance CreateFlameAccelerate()
    {
        return new CardInstance(
            new CardData(
                "flame_accelerate",
                "Flame Accelerate",
                CardCategory.Skill,
                1,
                "Draw cards equal to the target's Burn stacks.",
                new List<CardEffectData>(),
                new List<CardTag> {
                CardTag.Burn,
                CardTag.Draw
                }));
    }

    public static CardInstance CreateVulnerable()
    {
        return new CardInstance(
            new CardData(
                "vulnerable",
                "Vulnerable",
                CardCategory.Skill,
                1,
                "Apply 2 Vulnerable.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Vulnerable
                }));
    }

    public static CardInstance CreateSpotWeakness()
    {
        return new CardInstance(
            new CardData(
                "spot_weakness",
                "Spot Weakness",
                CardCategory.Attack,
                1,
                "Deal 7 damage. If target is Vulnerable, trigger once more.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Vulnerable,
                CardTag.Damage
                }));
    }

    public static CardInstance CreateTaunt()
    {
        return new CardInstance(
            new CardData(
                "taunt",
                "Taunt",
                CardCategory.Skill,
                1,
                "Gain 3 Block. Apply 2 Vulnerable.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Block,
                CardTag.Vulnerable
                }));
    }

    public static CardInstance CreatePoisonicFury()
    {
        return new CardInstance(
            new CardData(
                "poisonic_fury",
                "Poisonic Fury",
                CardCategory.Skill,
                1,
                "Apply 2 Vulnerable. If target is Poisoned, deal 5 damage.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Vulnerable,
                CardTag.Poison,
                CardTag.Damage
                }));
    }

    public static CardInstance CreatePressure()
    {
        return new CardInstance(
            new CardData(
                "pressure",
                "Pressure",
                CardCategory.Attack,
                2,
                "Deal 10 damage. If target is Vulnerable, reduce cost by 1.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Vulnerable,
                CardTag.Damage
                }));
    }

    public static List<CardInstance> CreateStarterDeck()
    {
        return new List<CardInstance>
        {
            CreateAttack(),
            CreateAttack(),
            CreateAttack(),
            CreateDefend(),
            CreateDefend(),
            CreateDefend(),
            CreateHeavyAttack()
        };
    }

    public static List<CardInstance> CreateRewardCardPool()
    {
        return new List<CardInstance>
        {
            CreatePoison(),
            CreateBurn(),
            CreateQuickStrike(),
            CreateToxicStrike(),
            CreateVenomGuard(),
            CreateAfterflare(),
            CreateEmptyArsenal(),
            CreateToxicEmber(),
            CreateHeatCharge(),
            CreateOverclockedFlames(),
            CreateToxicBurst(),
            CreateFlameAccelerate(),
            CreateToxicStacking(),
            CreateVulnerable(),
            CreateSpotWeakness(),
            CreateTaunt(),
            CreatePoisonicFury(),
            CreatePressure()
        };
    }
}