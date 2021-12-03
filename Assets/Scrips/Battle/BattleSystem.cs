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
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    //[SerializeField] GameController gameController;
    public UnityAction BattleOver;

    BattleState state;
    int currentAction; // 0:Fight, 1:Run
    int currentMove; // 0:左上, 1:右上

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        state = BattleState.Start;
        // モンスターの生成と描画
        playerUnit.Setup();
        enemyUnit.Setup();
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
        StartCoroutine(dialogBox.TypeDialog($"ゆけっ! {playerUnit.Pokemon.Base.Name} !"));
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    // PlayerMoveの実行
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        // 技を決定
        Move move = playerUnit.Pokemon.Moves[currentMove];
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
            BattleOver();
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
    }

    // PlayerActionでの行動を処理する
    void HandleActionSelection()
    {
        // 下を入力するとRUN上を入力するとFightになる
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                currentAction++;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                currentAction--;
            }
        }

        //色をつけてどちらを選択しているかわかるようにする
        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }
        }
    }

    // 0:左上, 1:右上
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
            {
                currentMove++;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }
        //色をつけてどちらを選択しているかわかるようにする
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
