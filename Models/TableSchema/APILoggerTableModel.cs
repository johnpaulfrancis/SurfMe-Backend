using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfMe.Models.TableSchema
{
    public class APILoggerTableModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }
        public int? InitiatedBy { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? Request { get; set; }
        public string? Response { get; set; }
        public DateTime LogDateInUTC { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public int? StatusCode { get; set; }
        public bool IsSuccess { get; set; }
    }
}
