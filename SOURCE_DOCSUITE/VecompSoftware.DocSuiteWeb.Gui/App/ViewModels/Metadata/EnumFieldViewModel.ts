import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');

class EnumFieldViewModel extends BaseFieldViewModel {
    Options: { [id: number]: string };
}

export = EnumFieldViewModel;