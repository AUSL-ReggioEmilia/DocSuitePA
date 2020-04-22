using System;
using System.Configuration;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace BiblosDS.Library.Common.StampaConforme
{
    [Serializable]
    [JsonObject]
    public class BoxLineConfig
    {
        public static readonly BaseFont DEFAULT_FONT = BaseFont.CreateFont();
        public const float DEFAULT_FONT_SIZE = 8.0f, DEFAULT_BOX_WIDTH = 10.0f, DEFAULT_BOX_HEIGHT = 10.0f, DEFAULT_SPACING = 5.0f;
        public const string DEFAULT_FORE_COLOR = "#00000000"/*BLACK*/;

        /// <summary>
        /// Testo della linea
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Se popolato renderizza le opzioni
        /// </summary>
        public string[] Options { get; set; }
        /// <summary>
        /// Opzione selezionata (se presente e richiesto).
        /// </summary>
        public string SelectedValue { get; set; }
        /// <summary>
        /// Se true imposta una checkbox
        /// </summary>
        public bool? TrueFalseBox
        {
            get
            {
                return (tfBox.HasValue) ? tfBox.Value : true;
            }
            set
            {
                tfBox = value;
            }
        }
        /// <summary>
        /// Colore del testo
        /// </summary>
        /// <example>#CC0033</example>
        public string ForeColor
        {
            get
            {
                return (!string.IsNullOrEmpty(foreColor)) ? foreColor : DEFAULT_FORE_COLOR;
            }
            set
            {
                foreColor = value;
            }
        }
        /// <summary>
        /// Dimensione del testo
        /// </summary>  
        /// <example>10px</example>
        public float? FontSize
        {
            get
            {
                return (fontSize.HasValue) ? fontSize.Value : DEFAULT_FONT_SIZE;
            }
            set
            {
                fontSize = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public float? Width
        {
            get
            {
                return (width.HasValue) ? width.Value : DEFAULT_BOX_WIDTH;
            }
            set
            {
                width = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public float? Height
        {
            get
            {
                return (height.HasValue) ? height.Value : DEFAULT_BOX_HEIGHT;
            }
            set
            {
                height = value;
            }
        }
        /// <summary>
        /// Il valore di spaziatura fra la casella ed il testo.
        /// </summary>
        public float? Spacing
        {
            get
            {
                return (spacing.HasValue) ? spacing.Value : DEFAULT_SPACING;
            }
            set
            {
                spacing = value;
            }
        }

        private float? width, height, fontSize, spacing;
        private string foreColor;
        private bool? tfBox;

        public BoxLineConfig()
        {
        }

        public BoxLineConfig(string msg, string[] opts, string sv, bool? tfBox, string fc, float? fs, float? w, float? h, float? s)
            : this()
        {
            initialize(msg, opts, sv, tfBox, fc, fs, w, h, s);
        }

        private void initialize()
        {
            var deser = JsonConvert.DeserializeObject<BoxLineConfig>(ConfigurationManager.AppSettings["stBoxLineConfig"]);
            initialize(deser.Message, deser.Options, deser.SelectedValue, deser.TrueFalseBox, deser.ForeColor, deser.FontSize, deser.Width, deser.Height, deser.Spacing);
        }

        private void initialize(string msg, string[] opts, string sv, bool? tfBox, string fc, float? fs, float? w, float? h, float? s)
        {
            Message = msg;
            Options = opts;
            SelectedValue = sv;
            TrueFalseBox = tfBox;
            ForeColor = fc;
            FontSize = fs;
            Width = w;
            Height = h;
            Spacing = s;
        }

        public static BoxLineConfig GetDefault()
        {
            var ret = new BoxLineConfig();
            ret.initialize();
            return ret;
        }
        /// <summary>
        /// Assegna i valori non impostati leggendoli dal file di configurazione.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static BoxLineConfig MergeWithConfig(BoxLineConfig source, log4net.ILog logger = null)
        {
            if (logger != null)
                logger.Debug("BoxLineConfig::MergeWithConfig - INIT");

            var key = ConfigurationManager.AppSettings["DefaultBoxLineConfig"];

            if (logger != null)
                logger.DebugFormat("BoxLineConfig::MergeWithConfig - readed key = {0}", key);

            if (string.IsNullOrEmpty(key))
            {
                if (logger != null)
                    logger.Debug("BoxLineConfig::MergeWithConfig - END");
                return source;
            }

            var retval = source ?? BoxLineConfig.GetDefault();

            try
            {
                var configured = JsonConvert.DeserializeObject<BoxLineConfig>(key);

                if (!retval.fontSize.HasValue)
                    retval.fontSize = configured.fontSize;
                if (!retval.height.HasValue)
                    retval.height = configured.height;
                if (!retval.spacing.HasValue)
                    retval.spacing = configured.spacing;
                if (!retval.tfBox.HasValue)
                    retval.tfBox = configured.tfBox;
                if (!retval.width.HasValue)
                    retval.width = configured.width;
                if (string.IsNullOrEmpty(retval.foreColor))
                    retval.foreColor = configured.foreColor;
                if (string.IsNullOrEmpty(retval.Message))
                    retval.Message = configured.Message;
                if (retval.Options == null || retval.Options.Length < 1)
                    retval.Options = configured.Options ?? new string[0];
                if (string.IsNullOrEmpty(retval.SelectedValue))
                    retval.SelectedValue = configured.SelectedValue;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error(ex);
                    logger.Debug("BoxLineConfig::MergeWithConfig - END");

                    return source;
                }
            }

            if (logger != null)
                logger.Debug("BoxLineConfig::MergeWithConfig - END");

            return retval;
        }
    }
}
