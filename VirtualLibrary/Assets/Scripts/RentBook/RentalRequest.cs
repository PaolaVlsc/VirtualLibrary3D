[System.Serializable]
public class RentalRequest
{
    public long user;
    public long book;

    public RentalRequest(long userId, long bookId)
    {
        user = userId;
        book = bookId;
    }
}
