using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL_ACCOUNTING.Models;

[Keyless]
public partial class IncomeStatement
{
    public int Seq { get; set; }

    [Column("Account Name")]
    [StringLength(48)]
    [Unicode(false)]
    public string? AccountName { get; set; }

    [Column(TypeName = "money")]
    public decimal Balance { get; set; }
}
