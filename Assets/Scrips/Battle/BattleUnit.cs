using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; }
    public BattleHud Hud { get => hud; }

    Vector3 originalPos;
    Color originalColor;
    Image image;

    // バトルで使うモンスターを保持

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = transform.localPosition;
        originalColor = image.color;
    }
    public void Setup(Pokemon pokemon)
    {
        // _baseからレベルに応じたモンスターを生成する
        // BattleSystemで使うからプロパティに入れる
        Pokemon = pokemon;

        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
        hud.SetData(pokemon);
        image.color = originalColor;
        PlayerEnterAnimation();
    }

    // 登場
    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
        {
            transform.localPosition = new Vector3(-850, originalPos.y);
        }
        else
        {
            transform.localPosition = new Vector3(850, originalPos.y);
        }
        // 戦闘時の位置
        transform.DOLocalMoveX(originalPos.x, 1f);
    }
    // 攻撃
    public void PlayerAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveX(originalPos.x+50f, 0.25f));
        sequence.Append(transform.DOLocalMoveX(originalPos.x, 0.2f));
    }

    // ダメージ
    public void PlayerHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
         sequence.Append(image.DOColor(originalColor, 0.1f));
    }
    // 戦闘不能
    public void PlayerFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0, 0.5f));
    }
}
