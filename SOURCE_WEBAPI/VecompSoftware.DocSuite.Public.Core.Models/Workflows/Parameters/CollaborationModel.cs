using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello dell'Unità Documentaria prodotta dalla collaborazione di firma
    /// </summary>
    public sealed class CollaborationModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello dell'unità documentaria prodotta dalla collaborazione di firma
        /// </summary>
        /// <param name="subject">Oggetto dell'unità documentaria</param>
        /// <param name="note">Note dell'unità documentaria</param>
        /// <param name="direction">Specifica se l'unità documentaria è in ingresso o in uscita <see cref="DocumentUnitDirection"/></param>
        /// <param name="documentUnitType">Tipologia dell'unità documentaria <see cref="DocumentUnitType"/></param>
        /// <param name="category">Classificatore o Titolario dell'unità documentaria</param>
        /// <param name="container">Classificatore o Titolario dell'unità documentaria</param>
        /// <param name="dueDate"> Data di promemoria della collaborazione</param>
        /// <param name="collaborationPriority">Priorità della collaborazione</param>
        public CollaborationModel(string subject, string note, DocumentUnitDirection direction,
            DocumentUnitType documentUnitType, CategoryModel category = null, ContainerModel container = null, 
            DateTimeOffset? dueDate = null, CollaborationPriorityType collaborationPriority = CollaborationPriorityType.Normal)
        {
            Subject = subject;
            Note = note;
            DueDate = dueDate;
            Category = category;
            Container = container;
            Direction = direction;
            DocumentUnitType = documentUnitType;
            CollaborationPriority = collaborationPriority;
            Metadatas = new HashSet<MetadataModel>();
            Contacts = new HashSet<ContactModel>();
            Sectors = new HashSet<DocSuiteSectorModel>();
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Oggetto dell'unità documentaria
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Note dell'unità documentaria
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// Proponente/Assegnatario dell'unità documentaria
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Data di promemoria della collaborazione
        /// </summary>
        public DateTimeOffset? DueDate { get; private set; }

        /// <summary>
        /// Classificatore o titolario dell'unità documentaria
        /// </summary>
        public CategoryModel Category { get; private set; }

        /// <summary>
        /// Contenitore dell'unità documentaria
        /// </summary>
        public ContainerModel Container { get; private set; }

        /// <summary>
        /// Specifica se l'unità documentaria è in ingresso o in uscita <see cref="DocumentUnitDirection"/>
        /// </summary>
        public DocumentUnitDirection Direction { get; private set; }

        /// <summary>
        /// Tipologia dell'unità documentaria <see cref="DocumentUnitType"/>
        /// </summary>
        public DocumentUnitType DocumentUnitType { get; private set; }

        /// <summary>
        /// Priorità della collaborazione <see cref="CollaborationPriorityType"/>
        /// </summary>
        public CollaborationPriorityType CollaborationPriority { get; private set; }

        /// <summary>
        /// Collezione coi metadati dell'archivio
        /// </summary>
        public ICollection<MetadataModel> Metadatas { get; private set; }

        /// <summary>
        /// Collezione coi contatti
        /// </summary>
        public ICollection<ContactModel> Contacts { get; private set; }


        /// <summary>
        /// Collezione dei settori
        /// </summary>
        public ICollection<DocSuiteSectorModel> Sectors { get; private set; }
        #endregion
    }
}
