using Microsoft.EntityFrameworkCore;

namespace EfCore.InMemoryHelpers.Test.Snippets
{
    public class MyDataContext : DbContext
    {
        #region dataContextCtor

        public MyDataContext(DbContextOptions options)
            :
            base(options)
        { }

        #endregion
    }
}