using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfMe.Models.TableSchema
{
    public class UserTableModel
    {
        [Key] // Primary key in DB
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment in DB
        public int UserId { get; set; }

        [Required] // NOT NULL in DB
        [MaxLength(50)] // Maximum length for Name in DB
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string LoginName { get; set; }

        [Required]
        [MaxLength(500)]
        public string Password { get; set; }

        public DateTime? CreatedOn { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
