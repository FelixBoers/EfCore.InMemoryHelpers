using Microsoft.EntityFrameworkCore;

public class MyDataContext : DbContext
{
    public MyDataContext(DbContextOptions options) :
        base(options)
    {
    }
}