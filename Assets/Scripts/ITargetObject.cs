using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Google.Rpc.Context.AttributeContext.Types;

public interface ITargetObject
{
    public void Activate();

    public void Deactivate();

    void Operate();
}
