using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL_ACCOUNTING.Models;

[Keyless]
public partial class BalanceSheet
{
    [StringLength(12)]
    [Unicode(false)]
    public string AccountNum { get; set; } = null!;

    [StringLength(48)]
    [Unicode(false)]
    public string? Descrip { get; set; }

    [Column(TypeName = "money")]
    public decimal? Balance { get; set; }
}
