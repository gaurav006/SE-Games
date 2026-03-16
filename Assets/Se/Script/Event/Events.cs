namespace SEGames
{
    public struct RoomStateChangedEvent
    {
        public RoomState NewState;
    }

    public struct PuzzleCompleteEvent { }

    public struct TileTappedEvent
    {
        public int GridX;
        public int GridY;
    }

   
    public struct ProceedRequestedEvent { }
    public struct NewGameRequestedEvent { }
    public enum RoomState
    {
        Locked,
        PuzzleActive,
        PuzzleSolved,
        DecorationUnlocked,
        RoomComplete
    }
}
