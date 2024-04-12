public class PlayerInfo
{
    public long Id { get; private set; }
    public string Name { get; private set; }

    public PlayerInfo(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
