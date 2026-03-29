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
                CardEffectType.DealDamage,
                1,
                5,
                "Deal 5 damage."));
    }

    public static CardInstance CreateDefend()
    {
        return new CardInstance(
            new CardData(
                "defend_basic",
                "Defend",
                CardCategory.Skill,
                CardEffectType.GainBlock,
                1,
                6,
                "Gain 6 Block."));
    }

    public static CardInstance CreateHeavyAttack()
    {
        return new CardInstance(
            new CardData(
                "heavy_attack",
                "Heavy Attack",
                CardCategory.Attack,
                CardEffectType.DealDamage,
                2,
                10,
                "Deal 10 damage."));
    }

    public static CardInstance CreatePoison()
    {
        return new CardInstance(
            new CardData(
                "poison_basic",
                "Poison",
                CardCategory.Status,
                CardEffectType.ApplyPoison,
                1,
                1,
                "Apply 1 Poison."));
    }

    public static CardInstance CreateBurn()
    {
        return new CardInstance(
            new CardData(
                "burn_basic",
                "Burn",
                CardCategory.Status,
                CardEffectType.ApplyBurn,
                1,
                1,
                "Apply 1 Burn."));
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
            CreateHeavyAttack(),
            CreatePoison(),
            CreateBurn()
        };
    }
}