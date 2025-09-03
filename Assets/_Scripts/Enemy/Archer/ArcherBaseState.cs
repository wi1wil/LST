using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArcherBaseState
{
    public abstract void EnterState(ArcherStateManager archer);
    public abstract void UpdateState(ArcherStateManager archer);
    public abstract void ExitState(ArcherStateManager archer);
}
