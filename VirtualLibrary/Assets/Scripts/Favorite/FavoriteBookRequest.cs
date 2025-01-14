[System.Serializable]
public class FavoriteBookRequest
{
    public long userId;  // Match the API's expected field name
    public long bookId;  // Match the API's expected field name

    public FavoriteBookRequest(long userId, long bookId)
    {
        this.userId = userId;  // Ensure proper assignment
        this.bookId = bookId;
    }
}
