using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VecompSoftware.WebAPIManager
{
    public class ODATAQueryManager : IODATAQueryManager
    {
        #region [ Fields ]
        private IList<string> _filterExpressions;
        private IList<string> _sortingExpressions;
        private IList<string> _expandExpressions;
        private string _functionExpression;
        private string _skipExpression;
        private string _topExpression;
        #endregion

        #region [ Constant ]
        private const string ODATA_FILTER_COMMAND = "$filter=";
        private const string ODATA_SORTING_COMMAND = "$orderby=";
        private const string ODATA_EXPAND_COMMAND = "$expand=";
        private const string ODATA_SKIP_COMMAND = "$skip=";
        private const string ODATA_TOP_COMMAND = "$top=";
        private const string ODATA_AND_SEPARATOR = " and ";
        private const string ODATA_COMMA_SEPARATOR = ",";
        #endregion

        #region [ Properties ]
        public string FilterExpressions
        {
            get
            {
                return string.Concat(ODATA_FILTER_COMMAND, string.Join(ODATA_AND_SEPARATOR, _filterExpressions));
            }
        }

        public string SortingExpressions
        {
            get
            {
                return string.Concat(ODATA_SORTING_COMMAND, string.Join(ODATA_COMMA_SEPARATOR, _sortingExpressions));
            }
        }

        public string ExpandExpressions
        {
            get
            {
                return string.Concat(ODATA_EXPAND_COMMAND, string.Join(ODATA_COMMA_SEPARATOR, _expandExpressions));
            }
        }

        public string FunctionExpression
        {
            get
            {
                return _functionExpression;
            }
        }

        public string SkipExpression
        {
            get
            {
                return _skipExpression;
            }
        }

        public string TopExpression
        {
            get
            {
                return _topExpression;
            }
        }

        public bool HasExpressions
        {
            get
            {
                return _filterExpressions.Count > 0 || _sortingExpressions.Count > 0
                    || _expandExpressions.Count > 0 || !string.IsNullOrEmpty(_skipExpression)
                    || !string.IsNullOrEmpty(_topExpression);
            }
        }
        #endregion

        #region [ Constructor ]
        public ODATAQueryManager()
        {
            _filterExpressions = new List<string>();
            _sortingExpressions = new List<string>();
            _expandExpressions = new List<string>();
        }
        #endregion

        #region [ Methods ]
        public IODATAQueryManager Function(string function)
        {
            if (string.IsNullOrEmpty(function))
            {
                return this;
            }

            _functionExpression = function;
            return this;
        }

        public IODATAQueryManager Skip(int skip)
        {
            _skipExpression = string.Concat(ODATA_SKIP_COMMAND, skip);
            return this;
        }

        public IODATAQueryManager Top(int top)
        {
            _topExpression = string.Concat(ODATA_TOP_COMMAND, top);
            return this;
        }

        public IODATAQueryManager Expand(string expand)
        {
            if (string.IsNullOrEmpty(expand))
            {
                return this;
            }

            CreateExpandCommand(expand);
            return this;
        }

        public IODATAQueryManager Filter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return this;
            }

            CreateFilterCommand(filter);
            return this;
        }

        public IODATAQueryManager Sorting(string sorting)
        {
            if (string.IsNullOrEmpty(sorting))
            {
                return this;
            }

            CreateSortingCommand(sorting);
            return this;
        }

        public string Compile()
        {
            string compiledUrl = string.Empty;
            if (!string.IsNullOrEmpty(_functionExpression))
            {
                compiledUrl = string.Concat("/", _functionExpression);
            }

            if (!HasExpressions)
            {
                return compiledUrl;
            }

            if (_filterExpressions.Count > 0)
            {
                compiledUrl = ComposeExpression(compiledUrl, FilterExpressions);
            }

            if (_sortingExpressions.Count > 0)
            {
                compiledUrl = ComposeExpression(compiledUrl, SortingExpressions);
            }

            if (_expandExpressions.Count > 0)
            {
                compiledUrl = ComposeExpression(compiledUrl, ExpandExpressions);
            }

            if (!string.IsNullOrEmpty(_skipExpression))
            {
                compiledUrl = ComposeExpression(compiledUrl, _skipExpression);
            }

            if (!string.IsNullOrEmpty(_topExpression))
            {
                compiledUrl = ComposeExpression(compiledUrl, _topExpression);
            }

            return compiledUrl;
        }

        private string ComposeExpression(string baseUrl, string expression)
        {
            if (baseUrl.Contains("$"))
            {
                baseUrl += "&";
            }
            else
            {
                baseUrl += "?";
            }

            return string.Concat(baseUrl, expression);
        }

        private void CreateFilterCommand(string expression)
        {
            expression = expression.Replace(ODATA_FILTER_COMMAND, string.Empty);
            _filterExpressions.Add(expression);
        }

        private void CreateSortingCommand(string expression)
        {
            expression = expression.Replace(ODATA_SORTING_COMMAND, string.Empty);
            _sortingExpressions.Add(expression);
        }

        private void CreateExpandCommand(string expression)
        {
            if (expression.StartsWith(ODATA_EXPAND_COMMAND))
            {
                Regex regex = new Regex(Regex.Escape(ODATA_EXPAND_COMMAND));
                expression = regex.Replace(expression, string.Empty, 1);
            }

            _expandExpressions.Add(expression);
        }
        #endregion        
    }
}
