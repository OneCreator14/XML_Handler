using System.ComponentModel.DataAnnotations.Schema;


public class District
{
    [Column("district_id")]
    public int DistrictId { get; set; }

    [Column("district")]
    public string? Name { get; set; }

    [Column("district_gc")]
    public string? DistrictGc { get; set; }

    public string? Gender { get; set; }

    public string? Head { get; set; }

    [Column("head_dc")]
    public string? HeadDc { get; set; }

    public string? Department { get; set; }
}
