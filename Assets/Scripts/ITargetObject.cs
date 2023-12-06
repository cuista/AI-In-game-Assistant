using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

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
