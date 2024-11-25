using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class InvalidsToHead
{
    [Key]
    [Column("invalid_id")]
    public int InvalidId { get; set; }

    public string? Name { get; set; }

    [Column("birth_date")]
    public string? BirthDate { get; set; }

    public string? District { get; set; }
}
