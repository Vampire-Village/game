namespace VampireVillage.Network
{
    public enum NetworkMode
    {
        Offline,
        Client,
        Server
    }

    public enum RoomState
    {
        Lobby,
        Game
    }

    public enum NetworkCode
    {
        Success,
        HostFailedAlreadyInRoom,
        JoinFailedAlreadyInRoom,
        JoinFailedRoomDoesNotExist,
        JoinFailedRoomGameAlreadyStarted,
        StartFailedNotInARoom,
        StartFailedNotHost,
        StartFailedNotEnoughPlayers
    }
}
