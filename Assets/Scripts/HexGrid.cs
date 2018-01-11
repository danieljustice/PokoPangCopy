using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour
{

    public int width = 7;
    public int height = 6;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    //public Color touchedColor = Color.magenta;
    public Color[] colors;
    Canvas gridCanvas;
    HexCell[] cells;
    HexMesh hexMesh;
    InnerHexMesh innerHexMesh;
    Stack<HexCell> cellStack = new Stack<HexCell>();

    void Awake()
    {

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        innerHexMesh = GetComponentInChildren<InnerHexMesh>();
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
        innerHexMesh.Triangulate(cells);
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
        if(colors.Length > 0)
        {
            cell.color = colors[(int)Random.Range(0, colors.Length)];
            cell.outerColor = cell.color;
        }
        else
        {
            cell.color = defaultColor;
        }
        


        SetNeighbors(cell, x, z, i);

        //position overlay for development purposes
        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

    void SetNeighbors(HexCell cell, int x, int z, int i)
    {
        //set neighbors
        //if not on the bottom row, then set the S && N neighbors
        if (i / width > 0)
        {
            cell.SetNeighbor(HexDirection.S, cells[i - width]);
        }
        //set SW and NE neighbors
        //if i am not all the way to the left of the board
        if (x > 0)
        {
            //if it is an even col
            if ((x & 1) == 0)
            {
                //if not on bottom row
                if (i / width > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            //else if odd col
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - 1]);
            }
        }

        //set SE and NW neighbors
        //if not on bottom row
        if (i / width > 0)
        {
            //if it is an even col
            if ((x & 1) == 0)
            {
                //if not on far right corner
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                    cell.SetNeighbor(HexDirection.NW, cells[i - 1]);
                }
            }
            if(x == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
            }
            //else
            //{
            //    cell.SetNeighbor(HexDirection.NW, cells[i - 1]);
            //}
        }
    }
   

    //void TouchCell(Vector3 position)
    //{
    //    position = transform.InverseTransformPoint(position);
    //    HexCoordinates coordinates = HexCoordinates.FromPosition(position);
    //    //Debug.Log("touched at " + coordinates.ToString());
    //    //super ugly equation that parses the hex's X,Z coordinates and 
    //    //finds its index on the cells array
    //    int index = (coordinates.Z+ coordinates.X/2) * height + coordinates.X;//+ coordinates.Z * width + coordinates.Z / 2;
    //    HexCell cell = cells[index];
    //    cell.color = touchedColor;
    //    hexMesh.Triangulate(cells);
    //}

    public void ColorCell(Vector3 position, Color color)
    {
        HexCell cell = GetCell(position);
        cell.color = color;
        cell.outerColor = color;
        hexMesh.Triangulate(cells);
        innerHexMesh.Triangulate(cells);
    }

    public Color GetCellColor(Vector3 position)
    {
        HexCell cell = GetCell(position);
        return cell.color;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        //super ugly equation that parses the hex's X,Z coordinates and 
        //finds its index on the cells array
        int index = (coordinates.Z + coordinates.X / 2) * width + coordinates.X;
        if (index < cells.Length && index  > 0)
        {
            return cells[index];
        }
        else
        {
            return null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else if (Input.GetMouseButtonUp(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            EndMove();   
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            PushToStack(hit.point);
        }
    }

    void PushToStack(Vector3 hitPoint)
    {
        HexCell hitCell = GetCell(hitPoint);
        if(hitCell == null)
        {
            return;
        }
        if(cellStack.Count == 0)
        {
            cellStack.Push(hitCell);
            hitCell.outerColor = defaultColor;
            hexMesh.Triangulate(cells);
            innerHexMesh.Triangulate(cells);
        }
        else
        {
            Color stackColor = cellStack.Peek().color;
            HexCell topCell = cellStack.Peek();
            //print(hitCell.color == stackColor);
            if (hitCell.color == stackColor)
            {
                if (topCell.IsNeighbor(hitCell))
                {
                    if (!cellStack.Contains(hitCell))
                    {
                        cellStack.Push(hitCell);
                        hitCell.outerColor = defaultColor;
                        hexMesh.Triangulate(cells);
                        innerHexMesh.Triangulate(cells);
                        print("pushed  " + hitCell.coordinates.ToString());
                    }else if (topCell.IsNeighbor(hitCell))
                    {
                        while(topCell != hitCell)
                        {
                            print(cellStack.Count);
                            topCell = cellStack.Pop();
                            topCell.outerColor = topCell.color;
                        }
                        
                    }
                }
            }
        }
    }

    void EndMove()
    {
        print("end " + cellStack.Count);
        if(cellStack.Count > 2)
        {

        }
        while(cellStack.Count != 0)
        {
            HexCell hexCell = cellStack.Pop();
            hexCell.outerColor = hexCell.color;
        }

        hexMesh.Triangulate(cells);
        innerHexMesh.Triangulate(cells);
    }

}