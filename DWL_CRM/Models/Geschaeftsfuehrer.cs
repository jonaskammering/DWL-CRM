using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class Geschaeftsfuehrer
{
    public int GeschaeftsfuehrerId { get; set; }

    public int PersonId { get; set; }

    public virtual Person Person { get; set; } = null!;
}
