using System.ComponentModel.DataAnnotations;

namespace HandlenettAPI.Configurations
{
    public class SlackSettings
    {
        [Required]
        public required string SlackBotUserOAuthToken { get; set; }
        [Required]
        public required string ContainerNameUserImages { get; set; }
        [Required]
        public required string BlobStorageUri { get; set; }
        public string BlobStoragePathIncludingContainer => BlobStorageUri + ContainerNameUserImages + "/";
    }
}
