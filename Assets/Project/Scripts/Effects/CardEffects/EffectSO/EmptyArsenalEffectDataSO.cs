using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Special/Empty Arsenal")]
public class EmptyArsenalEffectDataSO : CardEffectDataSO
{
    public int drawAmount = 2;

    public override ICardEffect CreateRuntimeEffect()
    {
        return new EmptyArsenalEffect(drawAmount);
    }
}
