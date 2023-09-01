using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL_ACCOUNTING.Models;

public partial class Journal
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("AccountID")]
    public int? AccountId { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string? JrnlType { get; set; }

    public int? TransNum { get; set; }

    [Column("DC")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Dc { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Posted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TransDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PostDate { get; set; }

    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Journals")]
    public virtual ChartOfAccount? Account { get; set; }
}
