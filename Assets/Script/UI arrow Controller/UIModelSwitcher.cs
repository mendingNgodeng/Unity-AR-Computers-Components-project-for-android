using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModelSwitcher : MonoBehaviour
{
   public void OnNextPressed()
    {
        if (SwipeChange.activeTarget != null)
        {
            SwipeChange.activeTarget.SwitchModelExternally(1);
        }
    }

    public void OnPrevPressed()
    {
        if (SwipeChange.activeTarget != null)
        {
            SwipeChange.activeTarget.SwitchModelExternally(-1);
        }
    }
}
