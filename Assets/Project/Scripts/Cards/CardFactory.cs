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
                }));
    }

    public static List<CardInstance> CreateStarterDeck()
    {
        return new List<CardInstance>
        {
            CreateAttack(),
            CreateAttack(),
            CreateDefend(),
            CreateHeavyAttack(),
            CreatePoison(),
            CreateBurn(),
            CreateQuickStrike(),
            CreateToxicStrike(),
            CreateVenomGuard()
        };
    }
}