import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import TextFieldViewModel = require('App/ViewModels/Metadata/TextFieldViewModel');
import EnumFieldViewModel = require('App/ViewModels/Metadata/EnumFieldViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');

class MetadataViewModel {
    TextFields: TextFieldViewModel[];
    NumberFields: BaseFieldViewModel[];
    DateFields: BaseFieldViewModel[];
    BoolFields: BaseFieldViewModel[];
    EnumFields: EnumFieldViewModel[];
    DiscussionFields: DiscussionFieldViewModel[];

    constructor() {
        this.TextFields = new Array<TextFieldViewModel>();
        this.NumberFields = new Array<BaseFieldViewModel>();
        this.DateFields = new Array<BaseFieldViewModel>();
        this.BoolFields = new Array<BaseFieldViewModel>();
        this.EnumFields = new Array<EnumFieldViewModel>();
        this.DiscussionFields = new Array<DiscussionFieldViewModel>();
    }
}

export = MetadataViewModel;