using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    public partial class FormCaricamento : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public int ValoreCorrente
        {
            get
            {
                return this.radPb.Value1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int ValoreMinimo
        {
            get
            {
                return this.radPb.Minimum;
            }
            set
            {
                this.radPb.Minimum = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int ValoreMassimo
        {
            get
            {
                return this.radPb.Maximum;
            }
            set
            {
                this.radPb.Maximum = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Incremento
        {
            get
            {
                return this.radPb.Step;
            }
            set
            {
                this.radPb.Step = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Titolo
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DescrizioneOperazione
        {
            get
            {
                return this.descrOp;
            }
            set
            {
                this.radPb.Text = value;
                this.descrOp = value;
                this.radLstMsg.Items.Add(value);
            }
        }
        /// <summary>
        /// Eventi annullato / completato.
        /// </summary>
        public event EventHandler OperazioneAnnullata, OperazioneCompletata;

        private bool operazioneAnnullata;
        private string descrOp;

        [Obsolete("Usare il costruttore esteso.", false)]
        public FormCaricamento()
        {
            InitializeComponent();
        }

        public FormCaricamento(string descrOperazione, string titolo = "", bool operazioneAnnullabile = true, int minVal = 0, int maxVal = 100, int step = 1)
        {
            this.Init(descrOperazione, titolo, operazioneAnnullabile, minVal, maxVal, step);
        }

        public void Reset()
        {
            this.radPb.Value1 = this.ValoreMinimo;
            this.radPb.ResetText();
            this.radLstMsg.Items.Clear();
            this.operazioneAnnullata = false;
        }

        public bool Incrementa()
        {
            bool esito = false;

            if (!this.operazioneAnnullata)
            {
                try
                {
                    this.radPb.Text = string.Format("{0} - {1} di {2}", this.DescrizioneOperazione, this.radPb.Value1, this.ValoreMassimo);
                    Application.DoEvents();

                    if (this.radPb.Value1 + this.Incremento <= this.ValoreMassimo)
                    {
                        this.radPb.Value1 += this.Incremento;
                        this.radPb.Text = string.Format("{0} - {1} di {2}", this.DescrizioneOperazione, this.radPb.Value1, this.ValoreMassimo);
                        Application.DoEvents();
                        esito = true;
                    }
                }
                catch { }
            }

            return esito;
        }

        protected void OnOperazioneAnnullata(object sender, EventArgs e)
        {
            if (this.OperazioneAnnullata != null)
                this.OperazioneAnnullata(sender, e);
        }

        protected void OnOperazioneCompletata(object sender, EventArgs e)
        {
            if (this.OperazioneCompletata != null)
                this.OperazioneCompletata(sender, e);
        }

        private void Init(string descrOperazione, string titolo, bool operazioneAnnullabile, int minVal, int maxVal, int step)
        {
            InitializeComponent();

            this.Titolo = titolo;
            this.DescrizioneOperazione = descrOperazione;
            this.ValoreMinimo = minVal;
            this.ValoreMassimo = maxVal;
            this.Incremento = step;
            this.radBtnAnnulla.Visible = operazioneAnnullabile;
            this.operazioneAnnullata = false;
        }

        private void radBtnAnnulla_Click(object sender, EventArgs e)
        {
            this.operazioneAnnullata = true;
            this.Hide();
            this.OnOperazioneAnnullata(sender, e);
            Application.DoEvents();
        }
    }
}
