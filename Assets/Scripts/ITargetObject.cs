using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Google.Rpc.Context.AttributeContext.Types;

public interface ITargetObject
{
    public void Activate();

    public void Deactivate();

    public void Operate();
}

public class TargetObject : MonoBehaviour, ITargetObject
{
    public virtual void Activate() { }

    public virtual void Deactivate() { }

    public virtual void Operate() { }
}
