using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base; // 戦わせるモンスターをセットする
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    // バトルで使うモンスターを保持
    public void Setup()
    {
        // _baseからレベルに応じたモンスターを生成する
        // BattleSystemで使うからプロパティに入れる
        Pokemon = new Pokemon(_base, level);
        Image image = GetComponent<Image>();
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
    }
}
