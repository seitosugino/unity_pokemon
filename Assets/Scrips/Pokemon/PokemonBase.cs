using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// モンスターのデータ:外部から変更しない(インスペクターだけ変更可能)
[CreateAssetMenu]
public class PokemonBase : ScriptableObject
{
    // 名前、説明、画像、タイプ、ステータス

    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;

    // 画像
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    // タイプ
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // ステータス:hp,at,df,sAT,sDF,sp
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    // 覚える技一覧
    [SerializeField] List<LearnableMove> learnableMoves;

    // 他のファイルからattackの値は取得できるが変更はできない
    public int MaxHP{　get => maxHP; }
    public int Attack{　get => attack; }
    public int Defense{　get => defense; }
    public int SpAttack{　get => spAttack; }
    public int SpDefense{　get => spDefense; }
    public int Speed{　get => speed; }

    public List<LearnableMove> LearnableMoves { get => learnableMoves; }
    public string Name { get => name; }
    public string Description { get => description; }
    public Sprite FrontSprite { get => frontSprite; }
    public Sprite BackSprite { get => backSprite; }
    public PokemonType Type1 { get => type1; }
    public PokemonType Type2 { get => type2; }
}

// 覚える技:どのレベルで覚えるか
[Serializable]
public class LearnableMove
{
    // ヒエラルキーで設定する
    [SerializeField] MoveBase _base;
    [SerializeField] int level;

    public MoveBase Base { get => _base; }
    public int Level { get => level; }
}

public enum PokemonType
{
    None,       // なし
    Normal,     //ノーマル
    Fire,       // 火
    Water,      // 水
    Electric,   // 電気
    Grass,      // 草
    Ice,        // 氷
    Fighting,   // 格闘
    Poison,     // 毒
    Ground,     // 地面
    Flying,     // 飛行
    Psychic,    // エスパー
    Bug,        // 虫
    Rock,       // 岩
    Ghost,      // ゴースト
    Dragon,     // ドラゴン
}