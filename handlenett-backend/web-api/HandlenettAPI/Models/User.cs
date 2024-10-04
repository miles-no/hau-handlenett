using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandlenettAPI.Models
{

    public class User : BaseEntity
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        //For å kunne ha det computed persisted i db
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Name { get; private set; }//=> FirstName + " " + LastName;
        public bool IsDeleted { get; set; }
        [MaxLength(50)]
        public string? SlackUserId { get; set; }
        [MaxLength(2048)]
        public string? ImageUrl { get; set; }
    }
}
