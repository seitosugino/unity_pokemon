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
                Id = ConditionID.Poison,
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
                Id = ConditionID.Burn,
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
                Id = ConditionID.Paralysis,
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
                Id = ConditionID.Freeze,
                Name = "凍り",
                StarMessage = "は凍った",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                        pokemon.CureStatus();
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
                Id = ConditionID.Sleep,
                Name = "眠り",
                StarMessage = "は眠った",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1,4);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は目を覚ました");
                        return true;
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は眠っている");
                    return false;

                }
            }
        },
        {
            ConditionID.Confusion,
            new Condition()
            {
                Id = ConditionID.Confusion,
                Name = "混乱",
                StarMessage = "は混乱した",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = Random.Range(1,5);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は混乱が解けた");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    if (Random.Range(1,3) == 1)
                    {
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は混乱している");
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} は訳も分からず自分を攻撃した");
                    return false;

                }
            }
        }
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
    Confusion,
}