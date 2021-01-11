import Serializable = require('App/Core/Serialization/Serializable');

class RadTreeNodeViewModel extends Serializable {
    text: string;
    value: any;
    imageUrl: string;
    expandMode: Telerik.Web.UI.ExpandMode;
    selected: boolean;
    expanded: boolean;
    attributes: any;

    initClassMapping(): void {
    }
}

export = RadTreeNodeViewModel;