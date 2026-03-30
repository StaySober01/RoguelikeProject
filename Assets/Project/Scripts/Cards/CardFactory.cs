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
                "Deal 5 damage.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 5)
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
                "Gain 6 Block.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.GainBlock, 6)
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
                "Apply 5 Poison.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.ApplyPoison, 5)
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
                "Deal 3 damage. Apply 3 Poison.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 3),
                    new CardEffectData(CardEffectType.ApplyPoison, 3)
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
                "Deal 4 damage. If target is Poisoned, gain 4 Block.",
                new List<CardEffectData>
                {
                    new CardEffectData(CardEffectType.DealDamage, 4)
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
                1,
                "Explosion damage is doubled this turn.",
                new List<CardEffectData>(),
                new List<CardTag>
                {
                CardTag.Burn
                }));
    }

    public static List<CardInstance> CreateStarterDeck()
    {
        return new List<CardInstance>
        {
            //CreateAttack(),
            //CreateAttack(),
            //CreateDefend(),
            //CreateHeavyAttack(),
            CreatePoison(),
            CreateBurn(),
            //CreateQuickStrike(),
            //CreateToxicStrike(),
            //CreateVenomGuard(),

            CreateAfterflare(),
            CreateEmptyArsenal(),
            CreateToxicEmber(),
            //CreateHeatCharge(),
            //CreateOverclockedFlames()
        };
    }
}