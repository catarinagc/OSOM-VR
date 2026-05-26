using System;

[System.Serializable]
public struct ImageKey
{
    public int year;
    public int hotspotId;
    public string direction;

    public override string ToString() => $"{year}_{hotspotId}_{direction}";

    public override bool Equals(object obj) =>
        obj is ImageKey other &&
        year == other.year &&
        hotspotId == other.hotspotId &&
        direction == other.direction;

    public override int GetHashCode() =>
        HashCode.Combine(year, hotspotId, direction);
}