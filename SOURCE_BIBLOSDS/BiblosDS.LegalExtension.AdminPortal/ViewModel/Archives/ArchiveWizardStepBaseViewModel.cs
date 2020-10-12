using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Archives
{
    public class ArchiveWizardStepBaseViewModel
    {
        public Guid IdArchive { get; set; }
        
        public string ArchiveName { get; set; }        

        public short ActiveStep { get; set; }

        [Required(ErrorMessage = "E' necessario selezionare almeno un attributo per definire la chiave primaria del documento")]
        public ICollection<string> SelectedPrimaryKeyAttributes { get; set; }
        
        [Required(ErrorMessage = "E' necessario inserire un percorso per la consevazione")]
        public string PathPreservation { get; set; }
        
        [Required(ErrorMessage = "E' necessario selezionare un attributo di tipo MainDate")]
        public string MainDateAttribute { get; set; }

        [Required(ErrorMessage = "E' necessario selezionare almeno un attributo per la conservazione")]
        public ICollection<string> SelectedPreservationAttributes { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsCompleteWithErrors { get; set; }
    }
}