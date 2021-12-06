using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.Poison,
            new Condition()
            {
                Name = "毒",
                StarMessage = "は毒になった",
                OnAfterTurn = (Pokemon pokemon) =>
                { 
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}は毒のダメージを受ける");
                }
            }
        },
        {
            ConditionID.Burn,
            new Condition()
            {
                Name = "火傷",
                StarMessage = "は火傷をおった",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}は火傷のダメージを受ける");
                }
            }
        },
    };
}

public enum ConditionID
{
    None,
    Poison,
    Burn,
    Sleep,
    Paralysis,
    Freeze,
}