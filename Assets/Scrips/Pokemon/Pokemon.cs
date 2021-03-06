using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// レベルに応じたステータスの違うモンスターを生成するクラス
// 注意:データのみを扱う:純粋なC#のクラス
[System.Serializable]
public class Pokemon
{
    // インスペクターからデータを設定できるようにする
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    // ベースとなるデータ
    public PokemonBase Base { get => _base; }
    public int Level { get => level; }

    public int HP { get; set; }
    // 使える技
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    // ステータスと追加ステータス
    public Dictionary<Stat, int> Stats { get; set; }
    public Dictionary<Stat, int> StatBoosts { get; set; }
    // ログを溜めておく変数を作る
    public Queue<string> StatusChanges { get; private set; }

    public Condition Status { get; private set; }
    public int StatusTime { get; set; }

    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }


    public bool HpChange { get; set; }

    Dictionary<Stat, string> statDic = new Dictionary<Stat, string>()
    {
        {Stat.Attack, "攻撃"},
        {Stat.Defense, "防御"},
        {Stat.SpAttack, "特攻"},
        {Stat.SpDefense, "特防"},
        {Stat.Speed, "素早さ"},
        {Stat.Accuracy, "命中率"},
        {Stat.Evasion, "回避率"},
    };

    // ステータス変化が起こった時に実行したいことを登録しておく
    public System.Action OnStatusChaged;

    // コントラクター:生成時の初期設定 => Init関数に変更
    public void Init()
    {
        StatusChanges = new Queue<string>();
        Moves = new List<Move>();
        // 覚える技の設定:覚える技のレベル以上ならMoveに追加
        foreach (LearnableMove learnableMove in Base.LearnableMoves)
        {
            if (Level >= learnableMove.Level)
            {
                // 技を覚える
                Moves.Add(new Move(learnableMove.Base));
            }
            // ４つ以上の技は使えない
            if (Moves.Count >=4)
            {
                break;
            }
        }

        CalculateStats();
        HP = MaxHP;

        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
        VolatileStatus = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level)/ 100f) + 10 + Level;
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
        {
            // 強化
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        }
        else
        {
            // 弱体化
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }
        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (StatBoost statBoost in statBoosts)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}の{statDic[stat]}が上がった");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}の{statDic[stat]}が下がった");
            }
        }
    }
    // levelに応じたステータスを返すもの:プロパティ(+処理を加えることができる)
    //プロパティ
    public int MaxHP { get; private set; }
    public int Attack
    {
        get{ return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get{ return GetStat(Stat.Defense); }
    }
    public int SpAttack
    {
        get{ return GetStat(Stat.SpAttack); }
    }
    public int SpDefense
    {
        get{ return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get{ return GetStat(Stat.Speed); }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        // 急所
        float critical = 1f;
        // 6.25%
        if (Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }
        // 相性
        float type = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1)*TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);
        DamageDetails damageDetails = new DamageDetails
        {
            Fainted = false,
            Critical = critical,
            TypeEffectiveness = type
        };

        // 特殊技の場合の修正
        float attack = attacker.Attack;
        float defense = Defense;
        if (move.Base.Category == MoveCategory.Special)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;
        }

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level+10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack/Defense)+2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);
        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HpChange = true;
    }

    public Move GetRandomMove()
    {
        List<Move> movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int r =Random.Range(0, movesWithPP.Count);
        return Moves[r];
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            return;
        }
        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{Status.StarMessage}");

        OnStatusChaged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChaged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            return;
        }
        VolatileStatus = ConditionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StarMessage}");
    }

    public void CureVolatileStatus()
    {
        Status = null;
    }

    public bool OnBeforeMove()
    {
        bool canRunMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (Status.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (VolatileStatus.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        return canRunMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
