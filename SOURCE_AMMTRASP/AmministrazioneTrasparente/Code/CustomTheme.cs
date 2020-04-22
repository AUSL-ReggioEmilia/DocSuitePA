using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmministrazioneTrasparente.Code
{
    public class CustomTheme
    {
        public enum BrowserCondition
        {
            Less,
            Greater,
            LessOrEqual,
            GreaterOrEqual,
            Equals
        }

        private const string jsonConfigurationFile = "configuration.json";

        #region [ Properties ]
        public int Priority
        {
            get;
            set;

        }

        public string CssName
        {
            get;
            set;
        }

        public int? ExplorerVersion
        {
            get;
            set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public BrowserCondition? Condition
        {
            get;
            set;
        }
        #endregion

        #region [ Methods ]

        public IEnumerable<CustomTheme> GetThemes(string themeName)
        {
            var cssPath = String.Format("~/css/Themes/{0}/{1}", themeName, jsonConfigurationFile);
            var path = HttpContext.Current.Server.MapPath(cssPath);            
            if (!File.Exists(path))
                return Enumerable.Empty<CustomTheme>();

            List<CustomTheme> themes;
            using (var sr = new StreamReader(path))
            {
                var json = sr.ReadToEnd();
                themes = JsonConvert.DeserializeObject<List<CustomTheme>>(json);
            }

            return themes;
        }

        public string GetStylesheetLink(CustomTheme theme, string themeName)
        {
            var sb = new StringBuilder();
            if (theme.ExplorerVersion.HasValue)
            {
                switch (theme.Condition)
                {
                    case BrowserCondition.Equals:
                        sb.AppendFormat("<!--[if IE {0}]>", theme.ExplorerVersion);
                        break;
                    case BrowserCondition.Greater:
                        sb.AppendFormat("<!--[if gt IE {0}]>", theme.ExplorerVersion);
                        break;
                    case BrowserCondition.GreaterOrEqual:
                        sb.AppendFormat("<!--[if gte IE {0}]>", theme.ExplorerVersion);
                        break;
                    case BrowserCondition.Less:
                        sb.AppendFormat("<!--[if lt IE {0}]>", theme.ExplorerVersion);
                        break;
                    case BrowserCondition.LessOrEqual:
                        sb.AppendFormat("<!--[if lte IE {0}]>", theme.ExplorerVersion);
                        break;
                }                
            }

            sb.AppendFormat("<link href=\"css/Themes/{0}/{1}\" rel=\"stylesheet\" />", themeName, theme.CssName);
            if (theme.ExplorerVersion.HasValue)
            {
                sb.Append("<![endif]-->");
            }
            return sb.ToString();
        }
        #endregion
    }
}