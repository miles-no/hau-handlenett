using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HandlenettAPI.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        [MaxLength(0)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(0)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
