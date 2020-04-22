using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace BiblosDS.Library.Common.StampaConforme
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfDrawer
    {
        private static readonly BaseFont DEFAULT_FONT = BaseFont.CreateFont();

        private Dictionary<PdfLayout, object> layoutInstances;
        private PdfContentByte canvas;
        private BoxConfig msgConfig;
        private BoxLineConfig optCfg;

        public PdfLayout BeginLayout()
        {
            var retval = new PdfLayout(DEFAULT_FONT, optCfg.FontSize.Value);
            retval.LayoutCompleted += new EventHandler<PdfLayout.LayoutEventArgs>(LayoutCompleted);

            this.layoutInstances.Add(retval, new object());

            #region Questo perchè se il documento non ha pagine (non c'è nessuna tabella da scrivere), iText dà eccezione.

            this.canvas.BeginText();
            this.canvas.EndText();

            #endregion

            return retval;
        }

        protected PdfPCell CellStyle
        {
            get
            {
                var cConv = new System.Drawing.ColorConverter();
                return new PdfPCell
                {
                    Border = Rectangle.NO_BORDER,
                    BackgroundColor = new BaseColor((System.Drawing.Color)cConv.ConvertFromInvariantString(msgConfig.BackgroundColor)),
                    BorderColor = new BaseColor((System.Drawing.Color)cConv.ConvertFromInvariantString(msgConfig.BorderColor)),
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    NoWrap = !msgConfig.WrapContent.Value,
                };
            }
        }

        private Image createBoxTemplate(string message, string selectedValue = null, bool forceCheckedBox = false)
        {
            var totalWidth = optCfg.Width.Value + DEFAULT_FONT.GetWidthPoint(message ?? string.Empty, optCfg.FontSize.Value) + optCfg.Spacing.Value;
            float width = optCfg.Width.Value, height = optCfg.Height.Value;
            var template = this.canvas.CreateTemplate(totalWidth, height);
            template.SetLineWidth(1.0f);
            template.SetColorStroke(GrayColor.GRAYBLACK);
            template.SetColorFill(CellStyle.BackgroundColor);
            template.Rectangle(0.0f, 0.0f, width, height);
            template.FillStroke();
            //Controlla se l'opzione corrisponde a quella desiderata. In questo caso la casella va disegnata con la X.
            if (forceCheckedBox ||
                (message != null && selectedValue != null && selectedValue.Equals(message, StringComparison.InvariantCultureIgnoreCase)))
            {
                template.MoveTo(0.0f, 0.0f);
                template.LineTo(width, height);

                template.MoveTo(0.0f, height);
                template.LineTo(width, 0.0f);
                //Disegna effettivamente le linee ad X.
                template.Stroke();
            }

            template.BeginText();
            template.SetFontAndSize(DEFAULT_FONT, optCfg.FontSize.Value);
            template.SetColorFill(new BaseColor((System.Drawing.Color)new System.Drawing.ColorConverter().ConvertFromInvariantString(optCfg.ForeColor)));
            template.ShowTextAligned(Element.ALIGN_RIGHT, message ?? string.Empty, totalWidth, 0.0f, 0.0f);
            template.EndText();

            return Image.GetInstance(template);
        }

        private PdfPCell createCellTemplate(string msg, bool isOption = false, string selectedValue = null, bool forceCheckedBox = false)
        {
            PdfPCell retval;
            var cellaStile = this.CellStyle;

            if (isOption)
            {
                retval = new PdfPCell(this.createBoxTemplate(msg, selectedValue, forceCheckedBox));
            }
            else
            {
                var phrase = new Phrase(msg, new Font(DEFAULT_FONT, optCfg.FontSize.Value));
                retval = new PdfPCell(phrase);
            }

            retval.HorizontalAlignment = cellaStile.HorizontalAlignment;
            retval.VerticalAlignment = cellaStile.VerticalAlignment;
            retval.Border = cellaStile.Border;
            retval.BackgroundColor = cellaStile.BackgroundColor;
            retval.BorderColor = cellaStile.BorderColor;
            retval.NoWrap = cellaStile.NoWrap;

            return retval;
        }

        private PdfPTable drawTableBorders(ref PdfPTable table)
        {
            PdfPCell[] celle;
            PdfPCell cella;
            const int PRIMA_RIGA = 0, PRIMA_CELLA = 0;
            int ultimaRiga = table.Rows.Count - 1, ultimaCella;

            for (int idxRiga = 0; idxRiga < table.Rows.Count; idxRiga++)
            {
                celle = table.Rows[idxRiga].GetCells();
                ultimaCella = celle.Length - 1;

                for (int idxCella = 0; idxCella < celle.Length; idxCella++)
                {
                    cella = celle.ElementAt<PdfPCell>(idxCella); //Cella corrente.

                    if (idxCella == PRIMA_CELLA) //Prima cella?
                    {
                        cella.Border = Rectangle.LEFT_BORDER;
                    }
                    //In caso di tabelle da una sola riga ed una sola cella, la cella corrente è anche "l'ultima" cella.
                    if (idxCella == ultimaCella) //Ultima cella?
                    {
                        cella.Border = Rectangle.RIGHT_BORDER;
                    }

                    if (idxRiga == PRIMA_RIGA) //Prima riga?
                    {
                        cella.Border |= Rectangle.TOP_BORDER;
                    }
                    //In caso di tabelle da una sola riga, la cella corrente è anche "l'ultima" cella (le va disegnato il bordo anche in basso).
                    if (idxRiga == ultimaRiga) //Ultima riga?
                    {
                        cella.Border |= Rectangle.BOTTOM_BORDER;
                    }
                    cella.BorderWidth = msgConfig.BorderWidth.Value;
                }
            }

            return table;
        }

        private void LayoutCompleted(object sender, PdfLayout.LayoutEventArgs e)
        {
            var pdfLayout = sender as PdfLayout;
            var layout = this.layoutInstances
                .Where(x => x.Key.Equals(pdfLayout))
                .Single();

            Exception toThrow = null;

            lock (layout.Value)
            {
                try
                {
                    var numeroMaxOpzioni = e.MessagesWithOptions
                        .Max(x => x.Options.Length);

                    //PdfPTable tabella = new PdfPTable(new[] { e.MaxMessageLength, e.MaxOptionLength }), tabellaOpzione;
                    //PdfPTable tabella = new PdfPTable(new[] { 20.0f, 80.0f }), tabellaOpzione;
                    PdfPTable tabella = new PdfPTable(2), tabellaOpzione;
                    tabella.WidthPercentage = 100.0f;

                    PdfPCell cellaMessaggio, cellaOpzione;
                    int optCount;

                    foreach (var elemento in e.MessagesWithOptions)
                    {
                        cellaMessaggio = this.createCellTemplate(elemento.Message);
                        tabellaOpzione = new PdfPTable(numeroMaxOpzioni);
                        tabellaOpzione.WidthPercentage = 100.0f;

                        optCount = 0;

                        foreach (var opzione in elemento.Options)
                        {
                            cellaOpzione = this.createCellTemplate(opzione, true, elemento.SelectedValue, elemento.Options.Length < 2 && elemento.SelectedValue != null);
                            tabellaOpzione.AddCell(cellaOpzione);
                            optCount++;
                        }

                        for (; optCount < numeroMaxOpzioni; optCount++)
                        {
                            cellaOpzione = new PdfPCell(new Phrase());
                            cellaOpzione.Border = Rectangle.NO_BORDER;
                            tabellaOpzione.AddCell(cellaOpzione);
                        }

                        tabella.AddCell(cellaMessaggio);
                        tabella.AddCell(new PdfPCell(tabellaOpzione, this.CellStyle));
                    }
                    //Calcola le dimensioni effettive della form.
                    var w1 = 0.0f;
                    var w2 = 0.0f;

                    #region AMPIEZZA

                    var totWidth = e.MaxMessageLength + optCfg.Spacing.Value;
                    totWidth += (e.MaxOptionLength + optCfg.Width.Value + optCfg.Spacing.Value) * numeroMaxOpzioni + optCfg.Spacing.Value;
                    //Controlla se l'ampiezza minima è impostata o se l'ampiezza viene calcolata automaticamente.
                    if (msgConfig.Width.HasValue && totWidth < msgConfig.Width.Value) //Tiene conto dell'ampiezza minima solo se 
                    {
                        //CASO 1: casella singola senza opzioni.
                        if (e.MessagesWithOptions.All(x => x.Options == null || x.Options.Length < 2 || x.Options.All(o => (o ?? string.Empty).Trim().Length < 1)))
                        {
                            w1 = msgConfig.Width.Value - optCfg.Width.Value - optCfg.Spacing.Value;
                            w2 = optCfg.Width.Value + optCfg.Spacing.Value;
                        }
                        else //CASO 2: una o più caselle con o senza opzioni.
                        {
                            w2 = (e.MaxOptionLength + optCfg.Width.Value + optCfg.Spacing.Value) * numeroMaxOpzioni + optCfg.Spacing.Value;
                            w1 = Math.Abs(msgConfig.Width.Value - w2);
                        }
                    }
                    else //Ampiezza calcolata automaticamente in funzione delle dimensioni massime delle celle.
                    {
                        w1 = e.MaxMessageLength + optCfg.Spacing.Value;
                        w2 = (e.MaxOptionLength + optCfg.Width.Value + optCfg.Spacing.Value) * numeroMaxOpzioni + optCfg.Spacing.Value;
                    }

                    //Imposta l'ampiezza definitiva.
                    tabella.SetTotalWidth(new[] { w1, w2 });

                    #endregion
                    #region LARGHEZZA

                    var larghezzaTotale = tabella.CalculateHeights();

                    if (msgConfig.Height.HasValue && tabella.Rows.Count > 0 && larghezzaTotale < msgConfig.Height.Value)
                    {
                        //Aggiunge una riga vuota (due celle alte N).
                        var differenza = msgConfig.Height.Value - larghezzaTotale;
                        var cellaVuota = this.CellStyle;
                        //cellaVuota.MinimumHeight = differenza;
                        cellaVuota.FixedHeight = differenza;
                        tabella.AddCell(cellaVuota);
                        tabella.AddCell(cellaVuota);
                    }

                    #endregion

                    //Disegna i bordi...
                    tabella = this.drawTableBorders(ref tabella);
                    //...e scrive la tabella nel documento.
                    tabella.WriteSelectedRows(0, tabella.Rows.Count, msgConfig.X.Value, msgConfig.Y.Value, this.canvas);
                }
                catch (Exception ex)
                {
                    toThrow = ex;
                }
            }

            if (toThrow != null)
                throw toThrow;
        }

        public PdfDrawer(PdfContentByte contentByte, ref BoxConfig messageConfig, ref BoxLineConfig optionConfig)
        {
            if (contentByte == null)
                throw new ApplicationException("PdfContentByte nullo.");

            this.canvas = contentByte;
            this.canvas.SetGState(new PdfGState { FillOpacity = messageConfig.Opacity.Value, StrokeOpacity = messageConfig.Opacity.Value });

            this.layoutInstances = new Dictionary<PdfLayout, object>();
            this.msgConfig = messageConfig;
            this.optCfg = optionConfig;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PdfLayout : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public struct MessageAndOptions
        {
            public string Message, SelectedValue;
            public string[] Options;
        }
        /// <summary>
        /// 
        /// </summary>
        public class LayoutEventArgs : EventArgs
        {
            public MessageAndOptions[] MessagesWithOptions { get; private set; }
            public float MaxMessageLength { get; private set; }
            public float MaxOptionLength { get; private set; }

            public LayoutEventArgs()
            {
                MessagesWithOptions = new MessageAndOptions[0];
            }

            public LayoutEventArgs(MessageAndOptions[] msgWithOptions, float maxMsgLen, float maxOptLen)
            {
                this.MessagesWithOptions = msgWithOptions;
                this.MaxMessageLength = maxMsgLen;
                this.MaxOptionLength = maxOptLen;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LayoutEventArgs> LayoutCompleted, LayoutDiscarded;

        private bool wasDisposed, wasDiscarded, wasAlreadyDrawed;
        private List<MessageAndOptions> msgWithOptions;
        private BaseFont font;
        private float fontSize, lunghezzaMassimaMessaggio, lunghezzaMassimaOpzione;
        /// <summary>
        /// 
        /// </summary>
        public void Discard()
        {
            this.resetState();
            this.wasDiscarded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            if (this.checkState())
            {
                var args = new LayoutEventArgs(this.msgWithOptions.ToArray<MessageAndOptions>(),
                    this.lunghezzaMassimaMessaggio,
                    this.lunghezzaMassimaOpzione);

                this.OnLayoutCompleted(this, args);

                this.resetState();

                this.wasAlreadyDrawed = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="selectedValue"></param>
        /// <param name="options"></param>
        public void AddMessage(string message, string selectedValue = null, params string[] options)
        {
            if (this.checkState())
            {
                var toAdd = new MessageAndOptions { Message = (message ?? string.Empty), Options = (options ?? new string[0]), SelectedValue = selectedValue, };
                //Se non ci sono opzioni, siamo nel caso in cui deve essere visualizzata solamente una casella senza testo a fianco.
                if (toAdd.Options.Length < 1)
                    toAdd.Options = new[] { string.Empty };
                //Variabili per calcolare l'ampiezza del testo (messaggio ed eventuali opzioni) in base al font.
                var maxLunghezzaOpzione = 0.0f;
                var lunghezzaMessaggio = 0.0f;

                if (toAdd.Options != null)
                {
                    var opzioniConteggiabili = toAdd.Options
                        .Where(x => !string.IsNullOrEmpty(x));

                    if (opzioniConteggiabili.Count() > 0)
                    {
                        maxLunghezzaOpzione = opzioniConteggiabili
                            .Max(x => this.font.GetWidthPoint(x, this.fontSize));
                    }
                }

                if (!string.IsNullOrEmpty(toAdd.Message))
                {
                    lunghezzaMessaggio = this.font.GetWidthPoint(toAdd.Message, this.fontSize);
                }

                if (maxLunghezzaOpzione > this.lunghezzaMassimaOpzione)
                    this.lunghezzaMassimaOpzione = maxLunghezzaOpzione;

                if (lunghezzaMessaggio > this.lunghezzaMassimaMessaggio)
                    this.lunghezzaMassimaMessaggio = lunghezzaMessaggio;

                this.msgWithOptions.Add(toAdd);
            }
        }

        #region IDisposable Members
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!wasDisposed)
            {
                try
                {
                    if (this.checkState())
                    {
                        GC.SuppressFinalize(this);
                        this.Draw();
                    }
                }
                finally
                {
                    GC.ReRegisterForFinalize(this);
                }
            }
            wasDisposed = true;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnLayoutCompleted(PdfLayout sender, LayoutEventArgs args)
        {
            if (this.LayoutCompleted != null)
                this.LayoutCompleted(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        protected void OnLayoutDiscarded()
        {
            if (this.LayoutDiscarded != null)
                this.LayoutDiscarded(this, new LayoutEventArgs());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool checkState()
        {
            var canGoOn = true;

            if (this.wasDiscarded)
                throw new ApplicationException("Questo layout è stato scartato.");
            if (this.wasAlreadyDrawed)
                throw new ApplicationException("Questo layout è stato già disegnato.");
            if (this.wasDisposed)
                throw new ApplicationException("Questo layout è stato già elaborato.");

            return canGoOn;
        }
        /// <summary>
        /// 
        /// </summary>
        private void resetState()
        {
            this.wasDiscarded = this.wasDisposed = this.wasAlreadyDrawed = false;
            this.lunghezzaMassimaMessaggio = this.lunghezzaMassimaOpzione = 0.0f;
            this.msgWithOptions.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textFont"></param>
        /// <param name="textFontSize"></param>
        public PdfLayout(BaseFont textFont, float textFontSize)
        {
            this.wasDisposed = false;
            this.font = textFont;
            this.fontSize = textFontSize;
            this.lunghezzaMassimaMessaggio = this.lunghezzaMassimaOpzione = 0;
            this.msgWithOptions = new List<MessageAndOptions>();
        }
    }
}
