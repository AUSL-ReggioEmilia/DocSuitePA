using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using VecompSoftware.DocSuiteWeb.Common.Helpers;

namespace VecompSoftware.DocSuite.Public.WebAPI.Handlers
{
    [AttributeUsage(System.AttributeTargets.Method)]
    public class DSWAuthorizeAttribute : AuthorizeAttribute
    {
        #region [ Fields ]

        private readonly string _kind;
        private readonly DSWAuthorizeType _type;
        private readonly string[] _params;
        private const string _endWith = "OAuth2/";

        #endregion

        #region [ Constructor ]
        public DSWAuthorizeAttribute(DSWAuthorizeType type, DSWAuthorizeClaimType kind, string[] @params)
        {
            _kind = EnumHelper.GetDescription(kind);
            _params = @params;
            _type = type;
        }
        #endregion

        #region [ Properties ]
        public virtual string Kind => _kind;

        public virtual string[] Params => _params;

        public virtual DSWAuthorizeType AuthorizeType => _type;

        public override bool AllowMultiple => false;

        #endregion

        #region [ Methods ]
        public override void OnAuthorization(HttpActionContext context)
        {
            bool isValid = false;
            try
            {
                if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    ClaimsIdentity claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                    string rule = claimsIdentity.Claims.Single(f => f.Type == DSWAuthorizationServerProvider.CLAIM_ExternalViewer_AuthenticationRule).Value;
                    isValid = claimsIdentity.Claims.Single(f => f.Type == DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Kind).Value == Kind;
                    Dictionary<string, string> requestValues = null;
                    switch (AuthorizeType)
                    {
                        case DSWAuthorizeType.Restfull:
                            {
                                requestValues = context.Request.GetQueryNameValuePairs()
                                    .ToDictionary(x => x.Key, x => x.Value);
                                break;
                            }
                        case DSWAuthorizeType.OData:
                            {
                                requestValues = context.Request.ODataProperties().Path.Segments
                                    .OfType<OperationSegment>().SelectMany(f => f.Parameters.Where(p => p.Value is ConstantNode))
                                    .ToDictionary(x => x.Name, x => ((ConstantNode)x.Value).Value.ToString());
                                break;
                            }
                        default:
                            {
                                isValid = false;
                                break;
                            }
                    }
                    isValid &= Params.All(x => requestValues.Single(v => x.EndsWith(string.Concat(_endWith, v.Key))).Value == claimsIdentity.Claims.Single(c => c.Type == x).Value);
                    switch (rule)
                    {
                        case DSWAuthorizationServerProvider.VALUE_ExternalViewer_AuthenticationRule_OAuth2:
                            {
                                break;
                            }
                        case DSWAuthorizationServerProvider.VALUE_ExternalViewer_AuthenticationRule_Token:
                            {
                                isValid &= isValid;
                                break;
                            }
                        default:
                            {
                                isValid = false;
                                break;
                            }
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            if (!isValid)
            {
                context.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }
            base.OnAuthorization(context);
        }

        #endregion

    }
}