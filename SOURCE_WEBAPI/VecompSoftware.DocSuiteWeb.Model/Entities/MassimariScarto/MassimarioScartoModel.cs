using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.MassimariScarto
{
    public class MassimarioScartoModel
    {
        #region [Constructor]

        public MassimarioScartoModel() { }

        public MassimarioScartoModel(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }

        #endregion

        #region [Properties]

        public Guid? UniqueId { get; set; }

        public short? Code { get; set; }

        public string FullCode { get; set; }

        public short? ConservationPeriod { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public MassimarioScartoStatus Status { get; set; }

        public short MassimarioScartoLevel { get; set; }

        public string MassimarioScartoPath { get; set; }

        public string MassimarioScartoParentPath { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public Guid? FakeInsertId { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string LastChangedUser { get; set; }

        #endregion
    }
}
