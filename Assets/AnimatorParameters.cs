using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameters : StateMachineBehaviour
{
    public bool isEnded = true;

    public void SetEnded(bool flag)
    {
        isEnded = flag;
    }
}
