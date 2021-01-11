using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Desks
{
    [Serializable]
    public class DeskEndorsement
    {
        public DeskEndorsement()
            : base()
        { }
        public Guid? DeskDocumentId { get; set; }
        public Guid? IdChainBiblos { get; set; }
        public string DeskDocumentName { get; set; }
        public decimal? Version { get; set; }
        public bool? IsApprove { get; set; }
        /// <summary>
        /// Proprietà utilizzata per la griglia di pivoting.
        /// 1 = Approvata
        /// 0 = Rifiutata
        /// -1 = Nessuna Approvazione
        /// </summary>
        public int? IsApproveForAggregation 
        {
            get
            {
                if (!IsApprove.HasValue)
                    return -1;
                else
                    if (IsApprove.Value)
                        return 1;
                    else
                        return 0;
            }
        }
        public string AccountName { get; set; }
    }
}
