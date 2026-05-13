using Database;
using Microsoft.EntityFrameworkCore.Design;

namespace API.Database.Context;

public class DuckSoupDesignTimeFactory : IDesignTimeDbContextFactory<DuckSoup>
{
    public DuckSoup CreateDbContext(string[] args)
    {
        DuckContext.ConnectionStrings[typeof(DuckSoup)] =
            "data source=localhost,1433;initial catalog=DuckSoup;persist security info=True;User Id=sa;Password=123456;MultipleActiveResultSets=True;App=DuckSoupEntityFramework;Encrypt=False;";

        return new DuckSoup();
    }
}
