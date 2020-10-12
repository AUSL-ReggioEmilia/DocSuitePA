using System.Linq;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Adaptors
{
    public static class ConvertAdaptor
    {
        public static SubjectRegistryService.ObjectIdType ConvertToSubjectRegistryService(this RepositoryService.ObjectIdType self)
        {
            return new SubjectRegistryService.ObjectIdType()
            {
                value = self.value
            };
        }
        public static BackOfficeService.ObjectIdType ConvertToBackOfficeService(this RepositoryService.ObjectIdType self)
        {
            return new BackOfficeService.ObjectIdType()
            {
                value = self.value
            };
        }
        public static ObjectService.ObjectIdType ConvertToObjectService(this RepositoryService.ObjectIdType self)
        {
            return new ObjectService.ObjectIdType()
            {
                value = self.value
            };
        }
        public static DocumentService.ObjectIdType ConvertToDocumentService(this RepositoryService.ObjectIdType self)
        {
            return new DocumentService.ObjectIdType()
            {
                value = self.value
            };
        }
        public static OfficialBookService.ObjectIdType ConvertToOfficialBookService(this RepositoryService.ObjectIdType self)
        {
            return new OfficialBookService.ObjectIdType()
            {
                value = self.value
            };
        }
        public static DocumentService.ObjectIdType ConvertToDocumentService(this OfficialBookService.ObjectIdType self)
        {
            return new DocumentService.ObjectIdType()
            {
                value = self.value
            };
        }
        public static OfficialBookService.PrincipalIdType ConvertToOfficialBookService(this BackOfficeService.PrincipalIdType self)
        {
            return new OfficialBookService.PrincipalIdType()
            {
                value = self.value
            };
        }
        public static ObjectService.PrincipalIdType ConvertToObjectService(this BackOfficeService.PrincipalIdType self)
        {
            return new ObjectService.PrincipalIdType()
            {
                value = self.value
            };
        }
        public static SubjectRegistryService.PrincipalIdType ConvertToSubjectRegistryService(this BackOfficeService.PrincipalIdType self)
        {
            return new SubjectRegistryService.PrincipalIdType()
            {
                value = self.value
            };
        }
        public static DocumentService.PrincipalIdType ConvertToDocumentService(this BackOfficeService.PrincipalIdType self)
        {
            return new DocumentService.PrincipalIdType()
            {
                value = self.value
            };
        }
        public static BackOfficeService.PagingResponseType ConvertSubjectToBackOfficeResponseType(this SubjectRegistryService.queryResponse self)
        {
            return new BackOfficeService.PagingResponseType
            {
                hasMoreItems = self.@object.hasMoreItems,
                objects = self.@object.objects.Select(x => new BackOfficeService.ObjectResponseType
                {
                    objectId = new BackOfficeService.ObjectIdType { value = x.objectId.value },
                    properties = x.properties.Select(t => new BackOfficeService.PropertyType
                    {
                        queryName = new BackOfficeService.QueryNameType { className = t.queryName.className, propertyName = t.queryName.propertyName },
                        value = t.value
                    }).ToArray()
                }).ToArray()
            };
        }
        public static BackOfficeService.PagingResponseType ConvertObjectToBackOfficeResponseType(this ObjectService.queryResponse self)
        {
            return new BackOfficeService.PagingResponseType
            {
                hasMoreItems = self.@object.hasMoreItems,
                objects = self.@object.objects.Select(x => new BackOfficeService.ObjectResponseType
                {
                    objectId = new BackOfficeService.ObjectIdType { value = x.objectId.value },
                    properties = x.properties.Select(t => new BackOfficeService.PropertyType
                    {
                        queryName = new BackOfficeService.QueryNameType { className = t.queryName.className, propertyName = t.queryName.propertyName },
                        value = t.value
                    }).ToArray()
                }).ToArray()
            };
        }

    }

}
