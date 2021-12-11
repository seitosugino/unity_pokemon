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
    [SerializeField] bool anyHit;
    [SerializeField] int pp;
    [SerializeField] int priority;

    // カテゴリー(物理/特殊/ステータス変化)
    [SerializeField] MoveCategory category;
    // ターゲット
    [SerializeField] MoveTarget target;
    // ステータス変化のリスト:どのステータスwpどの程度変化させるか
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;

    // 他のファイル(Move.cs)から参照する為にプロパティを使う
    public string Name{　get => name; }
    public string Description{　get => description; }
    public PokemonType Type{　get => type; }
    public int Power{　get => power; }
    public int Accuracy{　get => accuracy; }
    public int PP{　get => pp; }
    public MoveCategory Category { get => category; }
    public MoveTarget Target { get => target; }
    public MoveEffects Effects { get => effects; }
    public List<SecondaryEffects> Secondaries { get => secondaries; }
    public int Priority { get => priority; }
    public bool AnyHit { get => anyHit; }
}

public enum MoveCategory
{
    Physical,
    Special,
    Stat,
}

public enum MoveTarget
{
    Foe,
    Self,
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    public List<StatBoost> Boosts { get => boosts; }
    public ConditionID Status { get => status; }
    public ConditionID VolatileStatus { get => volatileStatus; }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get => chance; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}