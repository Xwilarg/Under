namespace VarVarGamejam.Map
{
    public enum TileType
    {
        Pending, // Only used during map generation

        EmptyTaken, // Used to check the user doesn't go by the same path twice

        Empty,
        Wall,
        Entrance,
        Exit
    }
}
