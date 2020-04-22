using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Commons
{
    public class PrivacyLevelFacade
    {
        private readonly PrivacyLevelFinder _finder;

        public PrivacyLevelFacade()
        {
            _finder = new PrivacyLevelFinder(DocSuiteContext.Current.CurrentTenant);
        }

        public ICollection<PrivacyLevel> GetCurrentPrivacyLevels()
        {
            _finder.EnablePaging = false;
            ICollection<WebAPIDto<PrivacyLevel>> results = _finder.DoSearch();
            return results?.Select(f => f.Entity).ToList();
        }

        public ICollection<PrivacyLevel> GetAllowedPrivacyLevels(int? minValue, int? maxValue)
        {
            _finder.EnablePaging = false;
            _finder.MaximumLevel = maxValue;
            _finder.MinimumLevel = minValue;
            ICollection<WebAPIDto<PrivacyLevel>> results = _finder.DoSearch();
            return results?.Select(f => f.Entity).ToList();
        }

        public PrivacyLevel GetByLevel(int level)
        {
            _finder.Level = level;
            _finder.EnablePaging = false;
            ICollection<WebAPIDto<PrivacyLevel>> results = _finder.DoSearch();
            return results?.Select(f => f.Entity).FirstOrDefault();
        }
    }
}
