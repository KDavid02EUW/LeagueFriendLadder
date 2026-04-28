using LeagueFriendLadder.Api.Models;

public enum FriendshipStatus
{
    None,
    Pending,
    Accepted,
    Blocked
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