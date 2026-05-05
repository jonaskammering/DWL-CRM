using System;
using System.Collections.Generic;

namespace DWL_CRM.Models;

public partial class Rechnungsdaten
{
    public int RechnungsdatenId { get; set; }

    public int FirmaId { get; set; }

    public decimal? RechnungenGesamt { get; set; }

    public decimal? RechnungenOffen { get; set; }

    public DateOnly? LetzterZahlungseingang { get; set; }

    public virtual Firma Firma { get; set; } = null!;
}
