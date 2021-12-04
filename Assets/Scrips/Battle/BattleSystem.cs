using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy,
    PartyScreen,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    //[SerializeField] GameController gameController;
    public UnityAction BattleOver;

    BattleState state;
    int currentAction; // 0:Fight, 1:Bag
    int currentMove; // 0:左上, 1:右上
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        state = BattleState.Start;
        // モンスターの生成と描画
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        partyScreen.Init();
        // HUDの描画
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"野生の {enemyUnit.Pokemon.Base.Name} が現れた。");
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog($"どうする"));
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    void OpenPartyAction()
    {
        state = BattleState.PartyScreen;
        // パーティスクリーンを表示
        // ポケモンデータを反映
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playerParty.Pokemons);
    }

    // PlayerMoveの実行
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        // 技を決定
        Move move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}の{move.Base.Name}");
        playerUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        enemyUnit.PlayerHitAnimation();
        // ダメージ計算
        DamageDetails damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        // HP反映
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}は倒れた。");
            enemyUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            // gameController.EndBattle();
            BattleOver();
        }
        else
        {
            //それ以外ならEnemyMove
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        // 技を決定:ランダム
        Move move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name}の{move.Base.Name}");
        enemyUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        playerUnit.PlayerHitAnimation();
        // ダメージ計算
        DamageDetails damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        // HP反映
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name}は倒れた。");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.7f);
            // gameController.EndBattle();
            // 戦えるポケモンがいるならポケモン選択
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {
                BattleOver();
            }
            else
            {
                OpenPartyAction();
            }
        }
        else
        {
            //それ以外なら
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog($"急所に当たった");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog($"効果は抜群だ");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog($"効果は今ひとつ");
        }
    }
    // Zボタンを押すとMoveSelectorとMoveDetailsを表示する
    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    // PlayerActionでの行動を処理する
    void HandleActionSelection()
    {
        // 下を入力するとRUN上を入力するとFightになる
        // 0:Fight   1:Bag
        // 2:Pokemon 3:Run
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        //色をつけてどちらを選択しているかわかるようにする
        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }
            if (currentAction == 2)
            {
                OpenPartyAction();
            }
        }
    }

    // 0:左上, 1:右上
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        //色をつけてどちらを選択しているかわかるようにする
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           currentMember -= 2;
        }

        currentMove = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        // 選択中のモンスター名に色をつける
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // モンスターの決定
            Pokemon selectedMember = playerParty.Pokemons[currentMember];
            // 入れ替える:現在のキャラと戦闘不能は不可
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessage("瀕死のポケモンは選べません");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("同じポケモンは選べません");
                return;
            }

            // ポケモン選択画面を消す
            partyScreen.gameObject.SetActive(false);
            // 状態をBusyにする
            state = BattleState.Busy;
            // 入れ替えの処理をする
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            // 戻る
            partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        // 前のやつを下げる
        bool fainted = playerUnit.Pokemon.HP <= 0;
        if (!fainted)
        {
            yield return dialogBox.TypeDialog($"戻れ!{playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        // 新しいのを出す
        playerUnit.Setup(newPokemon);
        playerHud.SetData(playerUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        yield return dialogBox.TypeDialog($"ゆけっ! {playerUnit.Pokemon.Base.Name} !");
        if (fainted)
        {
            PlayerAction();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
}
