using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class StagingCsv
{
    public string? Firma { get; set; }

    public string? Ansprechpartner { get; set; }

    public string? Strasse { get; set; }

    public string? PlzOrt { get; set; }

    public string? Telefon { get; set; }

    public string? Email { get; set; }

    public string? Geschaeftsfuehrer { get; set; }

    public string? GfGeburtsdatum { get; set; }

    public string? Gruendungsdatum { get; set; }

    public string? Branche { get; set; }

    public string? Jahresumsatz { get; set; }

    public string? RechnungenGesamt { get; set; }

    public string? RechnungenOffen { get; set; }

    public string? LetzterZahlungseingang { get; set; }

    public string? Bemerkungen { get; set; }
}
