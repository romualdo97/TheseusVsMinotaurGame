using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public bool Started { get; protected set; } = false;
    public virtual void Execute() { Started = true; }
    public virtual void UndoExecute() { Started = true; }
    public virtual void Reset() { Started = false; }
    public abstract bool IsCompleted();
}
