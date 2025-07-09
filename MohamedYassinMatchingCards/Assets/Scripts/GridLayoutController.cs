using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutController : MonoBehaviour
{
    public int rows;
    public int columns;

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayout = GetComponent<GridLayoutGroup>();
    }

    public void ConfigureGrid(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float spacingX = gridLayout.spacing.x;
        float spacingY = gridLayout.spacing.y;

        float totalSpacingX = spacingX * (columns - 1);
        float totalSpacingY = spacingY * (rows - 1);

        float cellWidth = (width - totalSpacingX) / columns;
        float cellHeight = (height - totalSpacingY) / rows;

        float size = Mathf.Min(cellWidth, cellHeight);

        gridLayout.cellSize = new Vector2(size, size);
    }
}
