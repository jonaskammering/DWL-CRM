using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class Firma
{
    public int FirmaId { get; set; }

    public string Firmenname { get; set; } = null!;

    public string? Strasse { get; set; }

    public int OrtId { get; set; }

    public string? Branche { get; set; }

    public DateOnly? Gruendungsdatum { get; set; }

    public decimal? Jahresumsatz { get; set; }

    public string? Bemerkungen { get; set; }

    public virtual Ort Ort { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = new List<Person>();

    public virtual Rechnungsdaten? Rechnungsdaten { get; set; }
}
