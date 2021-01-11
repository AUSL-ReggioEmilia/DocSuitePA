using System;
using System.Configuration;
using Newtonsoft.Json;

namespace BiblosDS.Library.Common.StampaConforme
{
    [Serializable]
    [JsonObject]
    public class BoxConfig
    {
        public const float DEFAULT_BORDER_WIDTH = 1.0f, DEFAULT_X = 10.0f, DEFAULT_Y = 792.0f /*PageSize.A4.Top - 50*/, DEFAULT_OPACITY = 0.3f;
        public const int DEFAULT_PAGE_NUMBER = 1;
        public const string DEFAULT_BORDER_COLOR = "#000000" /*BLACK*/, DEFAULT_BACKGROUND_COLOR = "#FFFFFF"/*WHITE*/;
        /// <summary>
        /// Righe da renderizzare nel box
        /// </summary>
        public BoxLineConfig[] BoxLine;
        /// <summary>
        /// Posizione x del box
        /// </summary>
        public float? X
        {
            get
            {
                return (x.HasValue) ? x.Value : DEFAULT_X;
            }
            set
            {
                x = value;
            }
        }
        /// <summary>
        /// Posizione y del box
        /// </summary>
        public float? Y
        {
            get
            {
                return (y.HasValue) ? y.Value : DEFAULT_Y;
            }
            set
            {
                y = value;
            }
        }
        /// <summary>
        /// Lunghezza
        /// </summary>
        public float? Width { get; set; }
        /// <summary>
        /// Altezza
        /// </summary>
        public float? Height { get; set; }
        /// <summary>
        /// Flag che specifica se il contenuto della form va wrappato.
        /// </summary>
        public bool? WrapContent
        {
            get
            {
                return (wrapContent.HasValue) ? wrapContent.Value : false;
            }
            set
            {
                wrapContent = value;
            }
        }
        /// <summary>
        /// La pagina in cui dovrà essere disegnata la form.
        /// Di default è la prima pagina.
        /// </summary>
        public int? DrawingPageNumber
        {
            get
            {
                return (pageNumber.HasValue) ? pageNumber.Value : DEFAULT_PAGE_NUMBER;
            }
            set
            {
                pageNumber = value;
            }
        }
        /// <summary>
        /// Colore di background 
        /// </summary>
        /// <remarks>Formato esadecimale</remarks>
        /// <example>#CC0033</example>
        public string BackgroundColor
        {
            get
            {
                return (!string.IsNullOrEmpty(bgColor)) ? bgColor : DEFAULT_BACKGROUND_COLOR;
            }
            set
            {
                bgColor = value;
            }
        }
        /// <summary>
        /// Spessore del bordo.
        /// </summary>
        public float? BorderWidth
        {
            get
            {
                return (borderWidth.HasValue) ? borderWidth.Value : DEFAULT_BORDER_WIDTH;
            }
            set
            {
                borderWidth = value;
            }
        }
        /// <summary>
        /// Colore del bordo.
        /// </summary>
        public string BorderColor
        {
            get
            {
                return (!string.IsNullOrEmpty(borderColor)) ? borderColor : DEFAULT_BORDER_COLOR;
            }
            set
            {
                borderColor = value;
            }
        }
        /// <summary>
        /// Scrive le opzioni inline
        /// </summary>
        public bool? InlineOption
        {
            get
            {
                return (isInline.HasValue) ? isInline : true;
            }
            set
            {
                isInline = value;
            }
        }
        /// <summary>
        /// Valore di trasparenza del box e del testo all'interno (messaggi ed opzioni).
        /// </summary>
        public float? Opacity
        {
            get
            {
                return (opacity.HasValue) ? opacity.Value : DEFAULT_OPACITY;
            }
            set
            {
                opacity = value;
            }
        }

        private float? borderWidth, opacity, x, y;
        private int? pageNumber;
        private string borderColor, bgColor;
        private bool? isInline, wrapContent;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("La serializzazione dell'oggetto imposterà i valori di default alle varie proprietà che lo prevedono, rendendo inconsistente lo stato dell'oggetto in fase di deserializzazione.")]
        public static BoxConfig GetFromObject(object obj)
        {
            return GetFromJSon(JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static BoxConfig GetFromJSon(string json)
        {
            return JsonConvert.DeserializeObject<BoxConfig>(json);
        }
        /// <summary>
        /// Assegna i valori non impostati leggendoli dal file di configurazione.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BoxConfig MergeWithConfig(BoxConfig source, log4net.ILog logger = null)
        {
            if (logger != null)
                logger.Debug("BoxConfig::MergeWithConfig - INIT");

            var key = ConfigurationManager.AppSettings["DefaultBoxConfig"];

            if (logger != null)
                logger.DebugFormat("BoxConfig::MergeWithConfig - readed key = {0}", key);

            if (string.IsNullOrEmpty(key))
            {
                if (logger != null)
                    logger.Debug("BoxConfig::MergeWithConfig - END");
                return source;
            }

            var retval = source ?? new BoxConfig();

            try
            {
                var configured = JsonConvert.DeserializeObject<BoxConfig>(key);

                if (!retval.x.HasValue)
                    retval.x = configured.x;
                if (!retval.y.HasValue)
                    retval.y = configured.y;
                if (!retval.pageNumber.HasValue)
                    retval.pageNumber = configured.pageNumber;
                if (!retval.isInline.HasValue)
                    retval.isInline = configured.isInline;
                if (!retval.borderWidth.HasValue)
                    retval.borderWidth = configured.borderWidth;
                if (!retval.opacity.HasValue)
                    retval.opacity = configured.opacity;
                if (string.IsNullOrEmpty(retval.bgColor))
                    retval.bgColor = configured.bgColor;
                if (string.IsNullOrEmpty(retval.borderColor))
                    retval.borderColor = configured.borderColor;

                if (retval.BoxLine == null || retval.BoxLine.Length < 1)
                {
                    retval.BoxLine = new[] { BoxLineConfig.GetDefault() };
                }
                else
                {
                    BoxLineConfig line;
                    for (int idx = 0; idx < retval.BoxLine.Length; idx++)
                    {
                        line = retval.BoxLine[idx];
                        line = BoxLineConfig.MergeWithConfig(line, logger);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error(ex);
                    logger.Debug("BoxConfig::MergeWithConfig - END");
                }
                return source;
            }

            if (logger != null)
                logger.Debug("BoxConfig::MergeWithConfig - END");

            return retval;
        }
    }
}
