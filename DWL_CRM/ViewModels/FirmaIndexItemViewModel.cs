using DWL_CRM.Models;

namespace DWL_CRM.ViewModels;

public class FirmaIndexItemViewModel
{
    public required Firma Firma { get; init; }

    public string AbcKategorie { get; init; } = "-";
}
