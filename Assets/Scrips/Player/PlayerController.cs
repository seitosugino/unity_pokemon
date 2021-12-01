using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    bool isMoving;
    Vector2 input;

    Animator animator;

    // 壁判定のLayer
    [SerializeField] LayerMask solidObjectsLayer;
    // 草むら判定のLayer
    [SerializeField] LayerMask longGrassLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                Vector2 targetPos = transform.position;
                targetPos += input;
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
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
        isMoving = false;
        CheckForEncounters();
    }
    // targetPosに移動可能かを調べる関数
    bool IsWalkable(Vector2 targetPos)
    {
        // targetPosに半径0.2fの円のRayを飛ばしてぶつからなかった
        return !Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer);
    }
    // 円のRayを飛ばして草むらLayerに当たったらエンカウント
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer))
        {
            // ランダムエンカウント
            if (Random.Range(0,100)<10)
            {
                Debug.Log("モンスターに遭遇");
            }
        }
    }
}
