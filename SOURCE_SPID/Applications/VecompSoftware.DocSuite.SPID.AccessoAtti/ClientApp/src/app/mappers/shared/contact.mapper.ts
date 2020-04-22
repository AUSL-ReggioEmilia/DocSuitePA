import { Injectable } from '@angular/core';

import { BaseMapper } from '@app-mappers/base.mapper';
import { ContactModel, ContactType } from '@app-models/_index';
import { NewRequestViewModel } from '@app-viewmodels/requests/new-request.viewmodel';

@Injectable()
export class ContactMapper extends BaseMapper<NewRequestViewModel, ContactModel> {

    constructor() {
        super();
    }

    map(source: NewRequestViewModel): ContactModel {
        let model: ContactModel = <ContactModel>{};
        model.$type = 'VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.ContactModel, VecompSoftware.DocSuite.Public.Core.Models'
        model.ContactType = ContactType.Citizen;
        model.Description = source.Name.concat('|', source.Surname);
        model.LanguageCode = 'IT';
        model.ArchiveSection = 'Richiedente';
        model.City = source.City;
        model.Address = source.Address;
        model.ZipCode = source.ZipCode;
        model.CivicNumber = source.CivicNumber;
        model.TelephoneNumber = source.TelephoneNumber;
        model.FiscalCode = source.FiscalCode;
        model.BirthDate = source.DateOfBirth;
        model.BirthPlace = source.PlaceOfBirth;
        model.EmailAddress = source.Email;
        model.PECAddress = source.PEC;
        model.ExternalCode = source.ExternalCode;

        return model;
    }
}
