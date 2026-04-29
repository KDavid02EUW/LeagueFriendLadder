using LeagueFriendLadder.Api.Models;

public enum FriendshipStatus
{
    None = 0,
    Pending = 1,
    Accepted = 2,
    Blocked = 3
}

public class Friend
{
    public int Id { get; set; }

    public int SenderUserId { get; set; }
    public User Sender { get; set; }
    public int ReceiverUserId { get; set; }
    public User Receiver { get; set; }

    public FriendshipStatus Status { get; set; }
}