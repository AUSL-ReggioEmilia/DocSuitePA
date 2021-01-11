using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara
{
    public class MenuModel
    {
        public MenuModel()
        {
            UniqueId = Guid.NewGuid();
        }

        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public ICollection<MenuModel> SubMenu { get; set; }
    }
}
