import MetadataViewModel = require('App/ViewModels/Metadata/MetadataViewModel');
import DiscussionFieldViewModel = require('App/ViewModels/Metadata/DiscussionFieldViewModel');
import CommentFieldViewModel = require('App/ViewModels/Metadata/CommentFieldViewModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class ViewMetadataComments {
    discussionLabel: string;
    pageContentId: string;
    uscNotificationId: string;

    private pageContent: HTMLElement;
    private _serviceConfigurations: ServiceConfiguration[];
    private _domainUserService: DomainUserService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        let metadataModelSession: string = sessionStorage.getItem("CurrentMetadataValues");
        this.pageContent = $("#".concat(this.pageContentId))[0];
        if (metadataModelSession) {
            let model: MetadataViewModel = JSON.parse(metadataModelSession);
            let discussion: DiscussionFieldViewModel = model.DiscussionFields.filter(i => i.Label == this.discussionLabel)[0];
            let orderedComments: CommentFieldViewModel[] = discussion.Comments;

            for (let comment of discussion.Comments) {

                this._domainUserService.getUser(comment.Author,
                    (user: DomainUserModel) => {
                        if (user) {
                            this.createCommentControl(user, comment);
                        }
                    },
                    (exception: ExceptionDTO) => {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (exception && uscNotification && exception instanceof ExceptionDTO) {
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        }
                    }
                );
            }
        }
    }

    private createCommentControl(user: DomainUserModel, comment: CommentFieldViewModel) {
        let div: HTMLDivElement = document.createElement('div');
        div.style.padding = '10px';
        let authorDiv: HTMLDivElement = document.createElement('div');
        let commentImage: HTMLImageElement = document.createElement('img');
        commentImage.setAttribute('src', '../App_Themes/DocSuite2008/imgset16/user_comment.png');
        let authorLabel: HTMLLabelElement = document.createElement('label');
        authorLabel.style.marginLeft = '5px';
        authorLabel.innerText = user.DisplayName.concat(" - ", moment(comment.RegistrationDate).format("DD/MM/YYYY"));
        authorLabel.style.fontStyle = 'italic';
        authorDiv.appendChild(commentImage);
        authorDiv.appendChild(authorLabel);
        let commentDiv: HTMLDivElement = document.createElement('div');
        let commentLabel: HTMLLabelElement = document.createElement('label');
        commentLabel.innerText = comment.Comment;
        commentDiv.appendChild(commentLabel);
        div.appendChild(authorDiv);
        div.appendChild(commentDiv);
        this.pageContent.appendChild(div);
    }
}

export = ViewMetadataComments;