using System.ComponentModel.DataAnnotations;

namespace HandlenettAPI.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
