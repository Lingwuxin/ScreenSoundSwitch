using System.ComponentModel.DataAnnotations;
namespace ScreenSoundSwitch.WebAPI.Models
{
    public class ShareAudioFIle
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        public string Url { get; set; } = string.Empty;
        [Required]
        public string Album { get; set; } = string.Empty;
        [Required]
        public int? Review { get; set; } = 0;

    }
}
