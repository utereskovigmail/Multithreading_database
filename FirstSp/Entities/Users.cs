using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstSp.Entities;

[Table ("Users")]
public class User
{
    [Key]public int id { get; set; }
    [StringLength(50)] public string Name {get; set;}
    [StringLength(50)] public string Surname {get; set;}
    [StringLength(200)] public string? Photo {get; set;}
}