using System;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    /// <summary>
    /// Modello per la gestione dei nodi di contenitore nella treeview
    /// </summary>
    public class ContainerNodeData
    {
        public ContainerNodeData()
        {

        }

        public ContainerNodeData(Container container)
        {
            this.UniqueIdContainer = container.UniqueId;
            this.Id = container.Id.ToString();
            this.IsContainer = true;
            this.HasDocument = (container.DocmLocation != null);
            this.HasDocumentSeries = (container.DocumentSeriesLocation != null);
            this.HasProtocol = (container.ProtLocation != null);
            this.HasResolution = (container.ReslLocation != null);
            this.HasUDS = (container.UDSLocation != null);
            this.IsActive = Convert.ToBoolean(container.IsActive);
            this.Name = container.Name;
        }

        public int FieldId { get; set; }
        public int FieldParentId { get; set; }
        public string Id { get; set; }
        public bool IsContainer { get; set; }
        public bool HasDocument { get; set; }
        public bool HasProtocol { get; set; }
        public bool HasResolution { get; set; }
        public bool HasDocumentSeries { get; set; }
        public bool HasUDS { get; set; }
        public bool HasLocations
        {
            get { return HasDocument || HasProtocol || HasResolution || HasDocumentSeries || HasUDS; }
        }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool MissingSecurityGroup { get; set; }
        public Guid UniqueIdContainer { get; set; }
    }
}
