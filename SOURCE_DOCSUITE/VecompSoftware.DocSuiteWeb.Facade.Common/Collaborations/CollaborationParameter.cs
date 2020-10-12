using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

public class CollaborationParameter
{
    #region [ Fields ]
    private const string SIGNERS_EDIT_ENABLED = "SignersEditEnabled";
    private const string DOCUMENT_UNIT_DRAFT_EDITOR_ENABLED = "DocumentUnitDraftEditorEnabled";
    private const string BTNCHEKOUT_ENABLED = "BtnCheckoutEnabled";
    private const string POPUP_DOCUMENT_NOT_SIGNED_ALERT_ENABLED = "PopUpDocumentNotSignedAlertEnabled";

    private static string _templateName;
    private static IDictionary<string, IList<JsonParameter>> _collaborationTemplateJsonParameters;
    #endregion


    #region [ Constructor ]
    public CollaborationParameter(string templateName)
    {
        _templateName = templateName;
    }
    #endregion

    #region [ Properties ]
    private static IDictionary<string, IList<JsonParameter>> CollaborationTemplateJsonParameters
    {
        get
        {
            if (_collaborationTemplateJsonParameters == null)
            {
                _collaborationTemplateJsonParameters = new Dictionary<string, IList<JsonParameter>>();
            }
            return _collaborationTemplateJsonParameters;
        }
    }

    public bool SignersEditEnabled
    {
        get
        {
            return GetBoolean(SIGNERS_EDIT_ENABLED, false).Value;
        }
    }

    public bool DocumentUnitDraftEditorEnabled
    {
        get
        {
            return GetBoolean(DOCUMENT_UNIT_DRAFT_EDITOR_ENABLED, false).Value;
        }
    }
    public bool BtnCheckoutEnabled
    {
        get
        {
            return GetBoolean(BTNCHEKOUT_ENABLED, true).Value;
        }
    }

    public bool PopUpDocumentNotSignedAlertEnabled
    {
        get
        {
            return GetBoolean(POPUP_DOCUMENT_NOT_SIGNED_ALERT_ENABLED, false).Value;
        }
    }
    #endregion

    #region [ Methods ]
    public static IList<JsonParameter> GetCollaborationTemplateJsonParameters()
    {
        if (string.IsNullOrEmpty(_templateName))
        {
            return null;
        }
        if (CollaborationTemplateJsonParameters != null && CollaborationTemplateJsonParameters.ContainsKey(_templateName))
        {
            return CollaborationTemplateJsonParameters[_templateName];
        }
        else
        {
            ICollection<WebAPIDto<TemplateCollaboration>> results = WebAPIImpersonatorFacade.ImpersonateFinder(new TemplateCollaborationFinder(DocSuiteContext.Current.Tenants),
                    (impersonationType, finder) =>
                    {
                        finder.ResetDecoration();
                        finder.EnablePaging = false;
                        finder.Name = _templateName;
                        return finder.DoSearch();
                    });

            if (results != null && results.Count > 0 && !string.IsNullOrEmpty(results.First().Entity.JsonParameters))
            {
                IList<JsonParameter> parameters = JsonConvert.DeserializeObject<IList<JsonParameter>>(results.First().Entity.JsonParameters);
                CollaborationTemplateJsonParameters.Add(_templateName, parameters);
                return parameters;
            }
        }
        return null;
    }

    private T GetGeneric<T>(string keyName, T defaultValue, Func<JsonParameter, T> eval)
    {
        T ret = defaultValue;
        if (GetCollaborationTemplateJsonParameters() != null && GetCollaborationTemplateJsonParameters().Count > 0)
        {
            JsonParameter value = GetCollaborationTemplateJsonParameters().SingleOrDefault(x => x.Name.Equals(keyName));
            if (value != null)
            {
                ret = eval(value);
            }
        }
        return ret;
    }

    private string GetString(string keyName, string defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => x.ValueString);
    }

    private long? GetInt(string keyName, long? defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => x.ValueInt);
    }
    private short? GetShort(string keyName, short? defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => Convert.ToInt16(x.ValueInt));
    }

    private double? GetDoubl(string keyName, double? defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => x.ValueDouble);
    }

    private bool? GetBoolean(string keyName, bool defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => x.ValueBoolean);
    }

    private Guid? GetGuid(string keyName, Guid? defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => x.ValueGuid);
    }

    private DateTime? GetDateTime(string keyName, DateTime? defaultValue)
    {
        return GetGeneric(keyName, defaultValue, x => x.ValueDate);
    }

    public static void ResetCollaborationTemplateJsonParameters()
    {
        _collaborationTemplateJsonParameters = null;
    }
    #endregion
}
