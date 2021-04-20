using UnityEngine;

public interface UndoAction
{
	void Undo(); // Undo, and undo an undo
}

public class ParentAction : UndoAction
{
	public Transform obj { get; }
	private Transform oldParent;

	public ParentAction(Transform obj, Transform oldParent)
	{
		this.obj = obj;
		this.oldParent = oldParent;
	}

	public void Undo()
	{
		var temp = obj.parent;
		obj.parent = oldParent;
		oldParent = temp;
	}
}

public class MoveAction : UndoAction
{
	private Transform obj;
	private Vector3 pos;

	public MoveAction(Transform obj, Vector3 pos)
	{
		this.obj = obj;
		this.pos = pos;
	}

	public void Undo()
	{
		var temp = obj.position;
		obj.position = pos;
		pos = temp;
	}
}

public class RotateAction : UndoAction
{
	private Transform obj;
	private Quaternion rot;

	public RotateAction(Transform obj, Quaternion rot)
	{
		this.obj = obj;
		this.rot = rot;
	}

	public void Undo()
	{
		var temp = obj.rotation;
		obj.rotation = rot;
		rot = temp;
	}
}

public class ScaleAction : UndoAction
{
	private Transform obj;
	private Vector3 pos, scale;

	public ScaleAction(Transform obj, Vector3 pos, Vector3 scale)
	{
		this.obj = obj;
		this.pos = pos;
		this.scale = scale;
	}

	public void Undo()
	{
		var temp = obj.position;
		var temp2 = obj.localScale;
		obj.position = pos;
		obj.localScale = scale;
		pos = temp;
		scale = temp2;
	}
}