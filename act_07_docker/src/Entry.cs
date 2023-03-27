using System.ComponentModel.DataAnnotations.Schema;

namespace DockerExample; 

public class Entry : BaseEntity {
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }
  public string Title { get; set; }
  [Column(TypeName = "text")]
  public string Content { get; set; }
}
