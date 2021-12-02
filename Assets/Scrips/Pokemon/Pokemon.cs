using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// レベルに応じたステータスの違うモンスターを生成するクラス
// 注意:データのみを扱う:純粋なC#のクラス
public class Pokemon
{
    // ベースとなるデータ
    public PokemonBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    //使える技
    public List<Move> Moves { get; set; }

    // コントラクター:生成時の初期設定
    public Pokemon(PokemonBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHP;

        Moves = new List<Move>();
        // 覚える技の設定:覚える技のレベル以上ならMoveに追加
        foreach (LearnableMove learnableMove in pBase.LearnableMoves)
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
    }
    // levelに応じたステータスを返すもの:プロパティ(+処理を加えることができる)
    //プロパティ
    public int MaxHP
    {
        get{ return Mathf.FloorToInt((Base.MaxHP * Level)/ 100f) + 5; }
    }
    public int Attack
    {
        get{ return Mathf.FloorToInt((Base.Attack * Level)/ 100f) + 5; }
    }
    public int Defense
    {
        get{ return Mathf.FloorToInt((Base.Defense * Level)/ 100f) + 5; }
    }
    public int SpAttack
    {
        get{ return Mathf.FloorToInt((Base.SpAttack * Level)/ 100f) + 5; }
    }
    public int SpDefense
    {
        get{ return Mathf.FloorToInt((Base.SpDefense * Level)/ 100f) + 5; }
    }
    public int Speed
    {
        get{ return Mathf.FloorToInt((Base.Speed * Level)/ 100f) + 10; }
    }
}
