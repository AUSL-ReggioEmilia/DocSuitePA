using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BiblosDs.Document.AdminCentral.UControls.Pager
{
    public partial class UcPager : UserControl
    {
        private bool isInitializing;

        public delegate void PageChangingHandler(object sender, int newPageNumber);

        public event PageChangingHandler FirstPage, NexPage, PrevPage, LastPage, GotoPage;

        public int CurrentPage
        {
            get { return (int)this.radSpinPage.Value; }
        }

        public UcPager()
        {
            InitializeComponent();
            try
            {
                this.Initialize();
            }
            catch (Exception)
            {

            }
        }

        public void Initialize(int minPage = 0, int maxPage = 0, int currentPage = 0)
        {
            this.isInitializing = true;

            minPage = minPage < 0 ? 0 : minPage;
            maxPage = maxPage < minPage ? minPage : maxPage;
            currentPage = currentPage <= maxPage && currentPage > minPage ? currentPage : minPage;

            this.radSpinPage.Minimum = minPage;
            this.radSpinPage.Maximum = maxPage;
            this.radSpinPage.Value = currentPage;
            this.radSpinPage.Step = 1.0m;

            this.radLblPagesCount.Text = maxPage.ToString();

            var enablePrev = currentPage > this.radSpinPage.Minimum;
            var enableNext = currentPage < this.radSpinPage.Maximum;

            this.radBtnPrev.Enabled = enablePrev;
            this.radBtnNext.Enabled = enableNext;

            this.radBtnFirst.Enabled = enablePrev;
            this.radBtnLast.Enabled = enableNext;

            this.isInitializing = false;
        }

        protected void OnFirstPage(object sender, int pageNumber)
        {
            if (this.FirstPage != null)
                this.FirstPage(sender, pageNumber);
        }

        protected void OnPrevPage(object sender, int pageNumber)
        {
            if (this.PrevPage != null)
                this.PrevPage(sender, pageNumber);
        }

        protected void OnNextPage(object sender, int pageNumber)
        {
            if (this.NexPage != null)
                this.NexPage(sender, pageNumber);
        }

        protected void OnLastPage(object sender, int pageNumber)
        {
            if (this.LastPage != null)
                this.LastPage(sender, pageNumber);
        }

        protected void OnGotoPage(object sender, int pageNumber)
        {
            if (this.GotoPage != null)
                this.GotoPage(sender, pageNumber);
        }

        private void radBtnFirst_Click(object sender, EventArgs e)
        {
            this.radSpinPage.Value = this.radSpinPage.Minimum;
        }

        private void radBtnPrev_Click(object sender, EventArgs e)
        {
            this.radSpinPage.Value--;
        }

        private void radBtnNext_Click(object sender, EventArgs e)
        {
            this.radSpinPage.Value++;
        }

        private void radBtnLast_Click(object sender, EventArgs e)
        {
            this.radSpinPage.Value = this.radSpinPage.Maximum;
        }

        private void radSpinPage_ValueChanging(object sender, Telerik.WinControls.UI.ValueChangingEventArgs e)
        {
            if (!this.isInitializing)
            {
                var value = (decimal)e.NewValue;
                var pageNum = (int)value;

                if (value == this.radSpinPage.Minimum)
                {
                    this.OnFirstPage(sender, pageNum);
                }
                else if (value == this.radSpinPage.Maximum)
                {
                    this.OnLastPage(sender, pageNum);
                }
                else if (value == (decimal)e.OldValue - 1.0m)
                {
                    this.OnPrevPage(sender, pageNum);
                }
                else if (value == (decimal)e.OldValue + 1.0m)
                {
                    this.OnNextPage(sender, pageNum);
                }
                else
                {
                    this.OnGotoPage(sender, pageNum);
                }
            }
        }
    }
}
