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
        {
            ConditionID.Paralysis,
            new Condition()
            {
                Name = "麻痺",
                StarMessage = "は麻痺になった",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}は火傷のダメージを受ける");
                        return false;
                    }
                    return true;

                }
            }
        },
        {
            ConditionID.Freeze,
            new Condition()
            {
                Name = "凍り",
                StarMessage = "は凍った",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                        pokemon.Curestatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} の凍りが溶けた");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} の凍って動けない");
                    return false;

                }
            }
        },
        {
            ConditionID.Sleep,
            new Condition()
            {
                Name = "眠り",
                StarMessage = "は眠った",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.SleepTime = Random.Range(1,4);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.SleepTime <= 0)
                    {
                        pokemon.Curestatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は目を覚ました");
                        return true;
                    }
                    pokemon.SleepTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は眠っている");
                    return false;

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