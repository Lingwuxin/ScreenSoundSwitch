using Microsoft.EntityFrameworkCore;
using ScreenSoundSwitch.WebAPI.Models;

namespace ScreenSoundSwitch.WebAPI
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
