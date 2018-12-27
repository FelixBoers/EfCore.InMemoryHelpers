using Microsoft.EntityFrameworkCore;

public class MyDataContext : DbContext
{
    #region dataContextCtor

    public MyDataContext(DbContextOptions options) :
        base(options)
    {
    }

    #endregion
}