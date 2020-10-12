using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI;

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
            ICollection<WebAPIDto<PrivacyLevel>> results = WebAPIImpersonatorFacade.ImpersonateFinder(_finder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.EnablePaging = false;
                        return finder.DoSearch();
                    });

            return results?.Select(f => f.Entity).ToList();
        }

        public ICollection<PrivacyLevel> GetAllowedPrivacyLevels(int? minValue, int? maxValue)
        {
            ICollection<WebAPIDto<PrivacyLevel>> results = WebAPIImpersonatorFacade.ImpersonateFinder(_finder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.EnablePaging = false;
                        finder.MaximumLevel = maxValue;
                        finder.MinimumLevel = minValue;
                        return finder.DoSearch();
                    });

            return results?.Select(f => f.Entity).ToList();
        }

        public PrivacyLevel GetByLevel(int level)
        {
            ICollection<WebAPIDto<PrivacyLevel>> results = WebAPIImpersonatorFacade.ImpersonateFinder(_finder,
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.Level = level;
                        finder.EnablePaging = false;
                        return finder.DoSearch();
                    });

            return results?.Select(f => f.Entity).FirstOrDefault();
        }
    }
}
