using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Data.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider);
    }
}
