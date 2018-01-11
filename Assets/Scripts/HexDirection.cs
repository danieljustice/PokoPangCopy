using UnityEngine;

public enum HexDirection
{
    NE, N, SE, SW, S, NW
}


public static class HexDirectionExtensions
{

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
}