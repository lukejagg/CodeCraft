using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
	public enum FitType
	{
		Uniform,
		Width,
		FixedWidth,
		Height,
		FixedHeight,
	}

	public Vector2 spacing = Vector2.zero;
	public FitType fitType = FitType.Height;
	public int gridHeight;
	public int gridWidth;
	public Vector2 cellSize;
	public bool fitX;
	public bool fitY;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();

		var sqrt = Mathf.Sqrt(transform.childCount);
		switch (fitType)
		{
			case FitType.Uniform:
				fitX = true;
				fitY = true;
				gridHeight = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
				gridWidth = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
				break;
			case FitType.Width:
				fitX = true;
				fitY = true;
				gridWidth = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
				gridHeight = Mathf.CeilToInt(transform.childCount * 1f / gridWidth);
				break;
			case FitType.FixedWidth:
				gridHeight = Mathf.CeilToInt(transform.childCount * 1f / gridWidth);
				break;
			case FitType.Height:
				fitX = true;
				fitY = true;
				gridHeight = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
				gridWidth = Mathf.CeilToInt(transform.childCount * 1f / gridHeight);
				break;
			case FitType.FixedHeight:
				gridWidth = Mathf.CeilToInt(transform.childCount * 1f / gridHeight);
				break;
		}

		var cellWidth = rectTransform.rect.width / gridWidth - (spacing.x / gridWidth) * (gridWidth - 1) - (padding.left + padding.right + 0f) / gridWidth;
		var cellHeight = rectTransform.rect.height / gridHeight - (spacing.y / gridWidth) * (gridWidth - 1) - (padding.top + padding.bottom + 0f) / gridHeight;

		if (fitX) cellSize.x = cellWidth;
		if (fitY) cellSize.y = cellHeight;

		for (int i = 0; i < rectChildren.Count; i++)
		{
			var item = rectChildren[i];
			var rowCount = i / gridWidth;
			var columnCount = i % gridWidth;

			SetChildAlongAxis(item, 0, cellSize.x * columnCount + spacing.x * columnCount + padding.left, cellSize.x);
			SetChildAlongAxis(item, 1, cellSize.y * rowCount + spacing.y * rowCount + padding.top, cellSize.y);
		}
	}

	public override void CalculateLayoutInputVertical()
    {

    }


    public override void SetLayoutHorizontal()
    {
		
	}

    public override void SetLayoutVertical()
    {

    }
}
