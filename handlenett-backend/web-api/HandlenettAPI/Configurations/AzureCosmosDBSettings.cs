namespace HandlenettAPI.Configurations
{
    using System.ComponentModel.DataAnnotations;

    public class AzureCosmosDBSettings
    {
        [Required]
        public required string ConnectionString { get; set; }
        //required property: Compile time validation
        //required attribute: Runtime validation (deserialization, reflection ++ can buypass required property)

        [Required]
        public required string DatabaseName { get; set; }

        [Required]
        public required string ContainerName { get; set; }
    }

}
