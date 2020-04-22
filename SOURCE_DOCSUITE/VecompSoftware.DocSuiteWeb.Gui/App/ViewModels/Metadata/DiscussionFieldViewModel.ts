import BaseFieldViewModel = require('App/ViewModels/Metadata/BaseFieldViewModel');
import CommentFieldViewModel = require('App/ViewModels/Metadata/CommentFieldViewModel');

class DiscussionFieldViewModel extends BaseFieldViewModel {
    Comments: CommentFieldViewModel[];
}

export = DiscussionFieldViewModel;