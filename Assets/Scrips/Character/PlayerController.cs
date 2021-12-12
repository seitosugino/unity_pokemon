using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    Vector2 input;

    Character character;

    public UnityAction OnEncounted;
    public UnityAction<Collider2D> OnEnterrTrainersView;

    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    private void Awake()
    {
        character = GetComponent<Character>();
    }
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        // 向いている方向
        Vector3 faceDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        // 干渉する場所
        Vector3 InteractPos = transform.position + faceDirection;
        // 干渉する場所にRayを飛ばす
        Collider2D collider2D = Physics2D.OverlapCircle(InteractPos, 0.3f, GameLayers.Instance.InteractableLayer);
        if (collider2D)
        {
            collider2D.GetComponent<IInteractable>()?.Interact(transform.position);
        }
    }

    void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfTrainerView();
    }

    // 円のRayを飛ばして草むらLayerに当たったらエンカウント
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.LongGrassLayer))
        {
            // ランダムエンカウント
            if (Random.Range(0,100)<10)
            {
                Debug.Log("モンスターに遭遇");
                //gameController.StartBattle();
                OnEncounted();
                character.Animator.IsMoving = false;
            }
        }
    }

    void CheckIfTrainerView()
    {
        Collider2D trainerCollider2D = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.FovLayer);
        if (trainerCollider2D)
        {
            Debug.Log("トレーナーの視界に入った");
            character.Animator.IsMoving = false;
            OnEnterrTrainersView?.Invoke(trainerCollider2D);
        }
    }
}