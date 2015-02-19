using UnityEngine;

public class AIStateChangeEventArgs : System.EventArgs
{
	public string OldStateName = "";
	public string NewStateName = "";

	public AIStateChangeEventArgs (string oldStateName, string newStateName)
	{
		this.OldStateName = oldStateName;
		this.NewStateName = newStateName;
	}
	
}
