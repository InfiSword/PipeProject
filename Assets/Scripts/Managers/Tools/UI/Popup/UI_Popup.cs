using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    protected bool isCam = false;
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true, isCam);
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
