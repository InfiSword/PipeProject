using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
	protected bool isCam = false;
	public override void Init()
	{
		Managers.UI.SetCanvas(gameObject, false, isCam);
	}
}
