using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.Services;
using Entities = AmministrazioneTrasparente.SQLite.Entities;

namespace AmministrazioneTrasparente.Admin
{
    public partial class EditParameter : System.Web.UI.Page
    {
        #region [ Fields ]
        private readonly ParameterService _parameterService = new ParameterService();
        private readonly UserLogService _userLogService = new UserLogService();
        #endregion

        #region Properties

        public int ParameterId
        {
            get
            {
                var id = Request.QueryString["Id"];
                if (!String.IsNullOrEmpty(id))
                {
                    return int.Parse(id);
                }

                return -1;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Initialize();                
            }
        }

        private void Initialize()
        {
            Entities.Parameter parameterData = this._parameterService.GetParameterById(ParameterId);
            if(parameterData == null) return;

            KeyName.Text = parameterData.KeyName;
            Note.Text = parameterData.Note;
            KeyValue.Value = parameterData.KeyValue;
        }

        protected void Close_OnClick(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "cancel", "CancelEdit();", true);
        }

        protected void Save_OnClick(object sender, EventArgs e)
        {
            this._parameterService.UpdateParameter(ParameterId, KeyValue.Value);
            this._userLogService.AddLog(Security.Username, String.Format("Parametro {0} modificato dall'utente {1}", KeyName.Text, Security.Username));
            ClientScript.RegisterStartupScript(Page.GetType(), "update", "CloseAndRebind();", true);
        }
    }
}