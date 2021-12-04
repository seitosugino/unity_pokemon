using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IInteractableを継承しているクラスは必ずInterractを持っている
public interface IInteractable
{
    // 関数を宣言する
    public void Interact();
}
