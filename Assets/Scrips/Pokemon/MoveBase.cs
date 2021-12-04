using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    // 技のマスターデータ
    // 名前、詳細、タイプ、威力、正確性、PP

    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy; //命中
    [SerializeField] int pp;

    // 他のファイル(Move.cs)から参照する為にプロパティを使う
    public string Name{　get => name; }
    public string Description{　get => description; }
    public PokemonType Type{　get => type; }
    public int Power{　get => power; }
    public int Accuracy{　get => accuracy; }
    public int PP{　get => pp; }

    // 特殊技
    public bool IsSpecial
    {
        get
        {
            if (type == PokemonType.Fire || type == PokemonType.Water || type == PokemonType.Grass
                || type == PokemonType.Ice || type == PokemonType.Electric || type == PokemonType.Dragon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
