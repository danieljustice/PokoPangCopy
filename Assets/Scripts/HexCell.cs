using UnityEngine;

public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;

    public Color color;
    public Color outerColor;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public bool IsNeighbor(HexCell cell)
    {
        bool isNeighbor = false;
        for(int i = 0; i < neighbors.Length; i++)
        {
            if(cell == neighbors[i])
            {
                isNeighbor = true;
                break;
            }
        }
        return isNeighbor;
    }
}