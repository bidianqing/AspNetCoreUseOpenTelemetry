using Dapper.Contrib.Extensions;

namespace AspNetCoreUseOpenTelemetry.Models
{
    [Table("tb_user")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
