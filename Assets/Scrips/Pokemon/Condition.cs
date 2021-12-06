using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StarMessage { get; set; }

    public Action<Pokemon> OnAfterTurn;
}
