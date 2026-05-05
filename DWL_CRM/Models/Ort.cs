using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class Ort
{
    public int OrtId { get; set; }

    public string Plz { get; set; } = null!;

    public string Ortsname { get; set; } = null!;

    public virtual ICollection<Firma> Firmas { get; set; } = new List<Firma>();
}
