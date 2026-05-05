using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class Ansprechperson
{
    public int AnsprechpersonId { get; set; }

    public int PersonId { get; set; }

    public virtual Person Person { get; set; } = null!;
}
