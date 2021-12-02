using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // モンスターの生成と描画
        playerUnit.Setup();
        enemyUnit.Setup();
        // HUDの描画
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} apeared.");
        yield return new WaitForSeconds(1);
        dialogBox.EnableActionSelector(true);
    }
    // Zボタンを押すとMoveSelectorとMoveDetailsを表示する
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableDialogText(false);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableMoveSelector(true);
        }
    }
}
