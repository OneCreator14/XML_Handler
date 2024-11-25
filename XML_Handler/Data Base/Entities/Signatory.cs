using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Signatory
{
    [Key]
    [Column("signatory_id")]
    public int SignatoryId { get; set; }

    public string? Name { get; set; }

    public string? Post { get; set; }
}
