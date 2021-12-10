using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movePattern;
    [SerializeField] float timeBetweenPattern;

    Character character;
    NPCState state;
    int currentPattern;

    float idleTimer = 0;

    void Awake()
    {
        currentPattern = 0;
        state = NPCState.Idle;
        character = GetComponent<Character>();
    }

    // 干渉された時に実行
    public void Interact(Vector3 initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, OnDialogFinished));
        }
    }

    void OnDialogFinished()
    {
        idleTimer = 0;
        state = NPCState.Idle;
    }

    void Update()
    {
        if (DialogManager.Instance.IsShowing)
        {
            return;
        }
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0;
                StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walk;
        Vector3 oldPosition = transform.position;
        yield return character.Move(movePattern[currentPattern]);
        if (oldPosition != transform.position)
        {
            currentPattern = (currentPattern + 1) % movePattern.Count;
        }
        state = NPCState.Idle;
    }
}

public enum NPCState
{
    Idle,
    Walk,
    Dialog,
}