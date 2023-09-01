using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL_ACCOUNTING.Models;

[Table("Chart_of_Accounts")]
[Index("AccountNum", Name = "UQ__Chart_of__B9572BDB3849AA5C", IsUnique = true)]
public partial class ChartOfAccount
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(12)]
    [Unicode(false)]
    public string AccountNum { get; set; } = null!;

    [StringLength(48)]
    [Unicode(false)]
    public string? Descrip { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? AcctType { get; set; }

    [Column(TypeName = "money")]
    public decimal? Balance { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
}
