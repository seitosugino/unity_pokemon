using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam, // マップ移動
    Battle,
    Dialog,
    CutScene,
}

public class GameController : MonoBehaviour
{
    // ゲームの状態を管理
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    TrainerController trainer;

    public static GameController Instance { get; private set; }

    GameState state = GameState.FreeRoam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerController.OnEncounted += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        playerController.OnEnterrTrainersView += TriggerTrainerBattle;

        DialogManager.Instance.OnShowDialog += ShowDialog;
        DialogManager.Instance.OnCloseDialog += CloseDialog;
    }

    void TriggerTrainerBattle(Collider2D trainerCollider2D)
    {
        TrainerController trainer = trainerCollider2D.GetComponentInParent<TrainerController>();
        if (trainer)
        {
            state = GameState.CutScene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }
    }

    void ShowDialog()
    {
        state = GameState.Dialog;
    }

    void CloseDialog()
    {
        if (state == GameState.Dialog)
        {
            state = GameState.FreeRoam;
        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        trainer = null;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        // パーティと野生ポケモンの取得
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        // FindObjectOfType:シーンないから一致するコンポーネントを1つ取得する
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        this.trainer = trainer;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void EndBattle()
    {
        if (trainer)
        {
            trainer.BattleLost();
        }
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
