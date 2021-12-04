using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] Dialog dialog;
    // NPCをSpriteAnimatorを使ってアニメーションさせる
    [SerializeField] List<Sprite> sprites;

    SpriteAnimator spriteAnimator;

    void Start()
    {
        spriteAnimator = new SpriteAnimator(GetComponent<SpriteRenderer>(), sprites);
        spriteAnimator.Start();
    }

    // 干渉された時に実行
    public void Interact()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    void Update()
    {
        spriteAnimator.HandleUpdate();
    }
}