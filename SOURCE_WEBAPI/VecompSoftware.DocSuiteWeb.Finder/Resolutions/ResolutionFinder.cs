using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Resolutions
{
    /// <summary>
    /// Extending IRepository<Resolution>
    /// </summary>
    public static class ResolutionFinder
    {

        public static IQueryable<Resolution> GetByUniqueId(this IRepository<Resolution> repository, Guid uniqueIdResolution)
        {
            return repository.Query(x => x.UniqueId == uniqueIdResolution)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static IQueryable<Resolution> GetFullByUniqueId(this IRepository<Resolution> repository, Guid uniqueIdResolution)
        {
            return repository.Query(t => t.UniqueId == uniqueIdResolution)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(t => t.Container.ReslLocation)
                .SelectAsQueryable();
        }

        public static ICollection<ResolutionTableValuedModel> GetOnlinePublishedByType(this IRepository<Resolution> repository, byte idType, short onlinePublicationInterval)
        {

            short actualOnlineInterval = (short)(onlinePublicationInterval + 1);
            ICollection<ResolutionTableValuedModel> results = repository.ExecuteModelFunction<ResolutionTableValuedModel>(CommonDefinition.SQL_FX_Resolution_AlboPretorioResolutions,
                new QueryParameter(CommonDefinition.SQL_Param_Resolution_OnlinePublicationInterval, actualOnlineInterval),
                new QueryParameter(CommonDefinition.SQL_Param_Resolution_IdType, idType));
            results.OrderByDescending(r => r.PublishingDate.Value).ThenByDescending(r => r.InclusiveNumber);

            return results;
        }

        public static IQueryable<Resolution> GetExecutiveByType(this IRepository<Resolution> repository, int skip, int top, byte idType, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            IQueryable<Resolution> results = repository.Queryable(true).Where(r => r.EffectivenessDate.HasValue && r.IdType == idType && r.Status == ResolutionStatus.Active);
            if (year.HasValue)
            {
                results = results.Where(r => r.Year.HasValue && r.Year.Value == year.Value);
            }

            if (number.HasValue)
            {
                results = results.Where(r => r.Number.HasValue && r.Number.Value == number.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                results = results.Where(r => r.Object.Contains(subject));
            }

            if (!string.IsNullOrEmpty(proposer))
            {
                results = results.Where(r => r.ResolutionContacts.Any(c => c.Contact.Description.Replace("|", " ").Contains(proposer)));
            }

            if (!string.IsNullOrEmpty(adoptionDate))
            {
                DateTime date = DateTime.ParseExact(adoptionDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                results = results.Where(r => r.AdoptionDate.HasValue && r.AdoptionDate.Value == date);
            }

            return results.OrderByDescending(r => r.EffectivenessDate.Value).ThenByDescending(r => r.InclusiveNumber).Skip(skip).Take(top);
        }

        public static IQueryable<Resolution> GetPublishedByType(this IRepository<Resolution> repository, int skip, int top, byte idType, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            DateTime currentDate = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-3);
            IQueryable<Resolution> results = repository.Queryable(true).Where(r => r.PublishingDate.HasValue && (!r.EffectivenessDate.HasValue || r.EffectivenessDate.Value > currentDate)
                                                && r.IdType == idType && r.Status == ResolutionStatus.Active);
            if (year.HasValue)
            {
                results = results.Where(r => r.Year.HasValue && r.Year.Value == year.Value);
            }

            if (number.HasValue)
            {
                results = results.Where(r => r.Number.HasValue && r.Number.Value == number.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                results = results.Where(r => r.Object.Contains(subject));
            }

            if (!string.IsNullOrEmpty(proposer))
            {
                results = results.Where(r => r.ResolutionContacts.Any(c => c.Contact.Description.Replace("|", " ").Contains(proposer)));
            }

            if (!string.IsNullOrEmpty(adoptionDate))
            {
                DateTime date = DateTime.ParseExact(adoptionDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                results = results.Where(r => r.AdoptionDate.HasValue && r.AdoptionDate.Value == date);
            }

            return results.OrderByDescending(r => r.PublishingDate.Value).ThenByDescending(r => r.InclusiveNumber).Skip(skip).Take(top);
        }

        public static int CountExecutiveByType(this IRepository<Resolution> repository, byte idType, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            IQueryable<Resolution> results = repository.Queryable(true).Where(r => r.EffectivenessDate.HasValue && r.IdType == idType && r.Status == ResolutionStatus.Active);
            if (year.HasValue)
            {
                results = results.Where(r => r.Year.HasValue && r.Year.Value == year.Value);
            }

            if (number.HasValue)
            {
                results = results.Where(r => r.Number.HasValue && r.Number.Value == number.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                results = results.Where(r => r.Object.Contains(subject));
            }

            if (!string.IsNullOrEmpty(proposer))
            {
                results = results.Where(r => r.ResolutionContacts.Any(c => c.Contact.Description.Replace("|", " ").Contains(proposer)));
            }

            if (!string.IsNullOrEmpty(adoptionDate))
            {
                DateTime date = DateTime.ParseExact(adoptionDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                results = results.Where(r => r.AdoptionDate.HasValue && r.AdoptionDate.Value == date);
            }

            return results.Count();
        }

        public static int CountPublishedByType(this IRepository<Resolution> repository, byte idType, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            DateTime currentDate = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-3);
            IQueryable<Resolution> results = repository.Queryable(true).Where(r => r.PublishingDate.HasValue && (!r.EffectivenessDate.HasValue || r.EffectivenessDate.Value > currentDate)
                                                && r.IdType == idType && r.Status == ResolutionStatus.Active);
            if (year.HasValue)
            {
                results = results.Where(r => r.Year.HasValue && r.Year.Value == year.Value);
            }

            if (number.HasValue)
            {
                results = results.Where(r => r.Number.HasValue && r.Number.Value == number.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                results = results.Where(r => r.Object.Contains(subject));
            }

            if (!string.IsNullOrEmpty(proposer))
            {
                results = results.Where(r => r.ResolutionContacts.Any(c => c.Contact.Description.Replace("|", " ").Contains(proposer)));
            }

            if (!string.IsNullOrEmpty(adoptionDate))
            {
                DateTime date = DateTime.ParseExact(adoptionDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                results = results.Where(r => r.AdoptionDate.HasValue && r.AdoptionDate.Value == date);
            }

            return results.Count();
        }

        public static int CountByResolutionKind(this IRepository<Resolution> repository, Guid idResolutionKind, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.ResolutionKind.UniqueId == idResolutionKind);
        }
    }
}
