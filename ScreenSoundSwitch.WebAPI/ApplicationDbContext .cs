using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScreenSoundSwitch.WebAPI.Models;

namespace ScreenSoundSwitch.WebAPI
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<AudioFile> audios { get; set; }
        public DbSet<ShareAudioFIle> shareAudioFIles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
