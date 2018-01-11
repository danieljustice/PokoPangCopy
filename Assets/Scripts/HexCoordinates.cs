using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;
    public int X {
        get{
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }
    
    //can be calculated from the other two coordinates
    public int Y
    {
        get
        {
            //X + Y + Z = 0, so Y = -X - Z
            return -X - Z;
        }
    }
    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x, z-x/2);
    }
    //Override the to string method so we dont just print out the struct name
    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.outerRadius * 1.5f);
        float y =  (position.y - position.x/2)/(HexMetrics.innerRadius * 2f);
        //float z = position.z / (HexMetrics.innerRadius * 2f);
        
        float offset = position.z / (HexMetrics.innerRadius * 2f);
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-y-x);

        //if the rounding error is too great then recalculate x 
        //and z based off of the other two
        if (iX + iY + iZ != 0)
        {
            //Debug.LogWarning("rounding error!");
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }
        return new HexCoordinates(iX, iZ);
    }
}
