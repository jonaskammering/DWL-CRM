using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public int FirmaId { get; set; }

    public string? Titel { get; set; }

    public string? Vorname { get; set; }

    public string? Nachname { get; set; }

    public DateOnly? Geburtsdatum { get; set; }

    public string? Telefon { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Ansprechperson> Ansprechpeople { get; set; } = new List<Ansprechperson>();

    public virtual Firma Firma { get; set; } = null!;

    public virtual ICollection<Geschaeftsfuehrer> Geschaeftsfuehrers { get; set; } = new List<Geschaeftsfuehrer>();
}
