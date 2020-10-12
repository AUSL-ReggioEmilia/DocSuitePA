define(["require", "exports", "App/Services/Securities/DomainUserService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, DomainUserService, ServiceConfigurationHelper, ExceptionDTO, SessionStorageKeysHelper) {
    var ViewMetadataComments = /** @class */ (function () {
        function ViewMetadataComments(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        ViewMetadataComments.prototype.initialize = function () {
            var _this = this;
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var metadataModelSession = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_METADATA_VALUES);
            this.pageContent = $("#".concat(this.pageContentId))[0];
            if (metadataModelSession) {
                var model = JSON.parse(metadataModelSession);
                var discussion = model.DiscussionFields.filter(function (i) { return i.Label == _this.discussionLabel; })[0];
                var orderedComments = discussion.Comments;
                var _loop_1 = function (comment) {
                    this_1._domainUserService.getUser(comment.Author, function (user) {
                        if (user) {
                            _this.createCommentControl(user, comment);
                        }
                    }, function (exception) {
                        var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                        if (exception && uscNotification && exception instanceof ExceptionDTO) {
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        }
                    });
                };
                var this_1 = this;
                for (var _i = 0, _a = discussion.Comments; _i < _a.length; _i++) {
                    var comment = _a[_i];
                    _loop_1(comment);
                }
            }
        };
        ViewMetadataComments.prototype.createCommentControl = function (user, comment) {
            var div = document.createElement('div');
            div.style.padding = '10px';
            var authorDiv = document.createElement('div');
            var commentImage = document.createElement('img');
            commentImage.setAttribute('src', '../App_Themes/DocSuite2008/imgset16/user_comment.png');
            var authorLabel = document.createElement('label');
            authorLabel.style.marginLeft = '5px';
            authorLabel.innerText = user.DisplayName.concat(" - ", moment(comment.RegistrationDate).format("DD/MM/YYYY"));
            authorLabel.style.fontStyle = 'italic';
            authorDiv.appendChild(commentImage);
            authorDiv.appendChild(authorLabel);
            var commentDiv = document.createElement('div');
            var commentLabel = document.createElement('label');
            commentLabel.innerText = comment.Comment;
            commentDiv.appendChild(commentLabel);
            div.appendChild(authorDiv);
            div.appendChild(commentDiv);
            this.pageContent.appendChild(div);
        };
        return ViewMetadataComments;
    }());
    return ViewMetadataComments;
});
//# sourceMappingURL=ViewMetadataComments.js.map