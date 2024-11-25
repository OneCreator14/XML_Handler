using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Executor
{
    [Key]
    [Column("executor_id")]
    public int ExecutorId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }
}
