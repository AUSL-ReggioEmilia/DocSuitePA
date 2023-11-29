import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import TextFieldViewModel = require('App/ViewModels/Metadata/TextFieldViewModel');
import EnumFieldViewModel = require('App/ViewModels/Metadata/EnumFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');

class MetadataDesignerViewModel {
    SETIFieldEnabled: boolean;
    TextFields: TextFieldViewModel[];
    NumberFields: BaseFieldViewModel[];
    DateFields: BaseFieldViewModel[];
    BoolFields: BaseFieldViewModel[];
    EnumFields: EnumFieldViewModel[];
    DiscussionFields: DiscussionFieldViewModel[];
    ContactFields: BaseFieldViewModel[];
    constructor() {
        this.TextFields = new Array<TextFieldViewModel>();
        this.NumberFields = new Array<BaseFieldViewModel>();
        this.DateFields = new Array<BaseFieldViewModel>();
        this.BoolFields = new Array<BaseFieldViewModel>();
        this.EnumFields = new Array<EnumFieldViewModel>();
        this.DiscussionFields = new Array<DiscussionFieldViewModel>();
        this.ContactFields = new Array<BaseFieldViewModel>();
    }
}

export = MetadataDesignerViewModel;