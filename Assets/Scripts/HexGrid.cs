using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public int width = 7;
    public int height = 6;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    Canvas gridCanvas;
    HexCell[] cells;
    HexMesh hexMesh;

    void Awake()
    {

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        //allocate memory for our grid, keeping it a 1D array
        cells = new HexCell[height * width];
        //double loop through to create each cell, i will be the cell's index in the cells array
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }


    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    //x is horizontal cell position, z is vertical cell position, i is cell position in cells array
    void CreateCell(int x, int z, int i)
    {
        //create position of the cell
        Vector3 position;
        position.x = x * (HexMetrics.outerRadius * 1.5f);
        position.y = 0f;
        position.z = (z + x * .5f - x/2) * (HexMetrics.innerRadius * 2f);

        //instantiate cell, set parent to the grid, and put it in position
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        //position overlay for development purposes
        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

}