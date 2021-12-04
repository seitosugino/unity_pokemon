using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    // 移動とアニメーションを管理する
    [SerializeField] float moveSpeed;
    CharacterAnimator animator;

    public bool IsMoving { get; set; }
    public CharacterAnimator Animator { get => animator; }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVec, UnityAction OnMoveOver = null)
    {
        animator.MoveX = moveVec.x;
        animator.MoveY = moveVec.y;
        Vector3 targetPos = transform.position;
        targetPos += (Vector3)moveVec;
        if (!IsWalkable(targetPos))
        {
            yield break;
        }

        // 移動中は入力を受け付けたくない
        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed*Time.deltaTime
                );
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;
        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    // targetPosに移動可能かを調べる関数
    bool IsWalkable(Vector2 targetPos)
    {
        // targetPosに半径0.2fの円のRayを飛ばしてぶつからなかった
        return !Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer);
    }
}
