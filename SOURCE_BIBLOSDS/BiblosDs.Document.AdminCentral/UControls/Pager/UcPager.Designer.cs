namespace BiblosDs.Document.AdminCentral.UControls.Pager
{
    partial class UcPager
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radRootPanel = new Telerik.WinControls.UI.RadPanel();
            this.radBtnLast = new Telerik.WinControls.UI.RadButton();
            this.radBtnNext = new Telerik.WinControls.UI.RadButton();
            this.radLblPagesCount = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radSpinPage = new Telerik.WinControls.UI.RadSpinEditor();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radBtnPrev = new Telerik.WinControls.UI.RadButton();
            this.radBtnFirst = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radRootPanel)).BeginInit();
            this.radRootPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnLast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLblPagesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSpinPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnPrev)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnFirst)).BeginInit();
            this.SuspendLayout();
            // 
            // radRootPanel
            // 
            this.radRootPanel.Controls.Add(this.radBtnLast);
            this.radRootPanel.Controls.Add(this.radBtnNext);
            this.radRootPanel.Controls.Add(this.radLblPagesCount);
            this.radRootPanel.Controls.Add(this.radLabel1);
            this.radRootPanel.Controls.Add(this.radSpinPage);
            this.radRootPanel.Controls.Add(this.radLabel2);
            this.radRootPanel.Controls.Add(this.radBtnPrev);
            this.radRootPanel.Controls.Add(this.radBtnFirst);
            this.radRootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radRootPanel.Location = new System.Drawing.Point(0, 0);
            this.radRootPanel.Name = "radRootPanel";
            this.radRootPanel.Size = new System.Drawing.Size(447, 31);
            this.radRootPanel.TabIndex = 0;
            // 
            // radBtnLast
            // 
            this.radBtnLast.Image = global::BiblosDs.Document.AdminCentral.Properties.Resources.BindingNavigatorMoveLastItem_Image1;
            this.radBtnLast.Location = new System.Drawing.Point(381, 3);
            this.radBtnLast.Name = "radBtnLast";
            this.radBtnLast.Size = new System.Drawing.Size(62, 25);
            this.radBtnLast.TabIndex = 3;
            this.radBtnLast.Text = "La&st";
            this.radBtnLast.Click += new System.EventHandler(this.radBtnLast_Click);
            // 
            // radBtnNext
            // 
            this.radBtnNext.Image = global::BiblosDs.Document.AdminCentral.Properties.Resources.BindingNavigatorMoveNextItem_Image1;
            this.radBtnNext.Location = new System.Drawing.Point(313, 3);
            this.radBtnNext.Name = "radBtnNext";
            this.radBtnNext.Size = new System.Drawing.Size(62, 25);
            this.radBtnNext.TabIndex = 2;
            this.radBtnNext.Text = "&Next";
            this.radBtnNext.Click += new System.EventHandler(this.radBtnNext_Click);
            // 
            // radLblPagesCount
            // 
            this.radLblPagesCount.Location = new System.Drawing.Point(258, 7);
            this.radLblPagesCount.Name = "radLblPagesCount";
            this.radLblPagesCount.Size = new System.Drawing.Size(12, 18);
            this.radLblPagesCount.TabIndex = 4;
            this.radLblPagesCount.Text = "0";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(236, 7);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(16, 18);
            this.radLabel1.TabIndex = 3;
            this.radLabel1.Text = "of";
            // 
            // radSpinPage
            // 
            this.radSpinPage.Enabled = false;
            this.radSpinPage.Location = new System.Drawing.Point(183, 5);
            this.radSpinPage.Name = "radSpinPage";
            // 
            // 
            // 
            this.radSpinPage.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radSpinPage.ShowBorder = true;
            this.radSpinPage.Size = new System.Drawing.Size(47, 20);
            this.radSpinPage.TabIndex = 3;
            this.radSpinPage.TabStop = false;
            this.radSpinPage.ValueChanging += new Telerik.WinControls.UI.ValueChangingEventHandler(this.radSpinPage_ValueChanging);
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(146, 7);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(31, 18);
            this.radLabel2.TabIndex = 2;
            this.radLabel2.Text = "Page";
            // 
            // radBtnPrev
            // 
            this.radBtnPrev.Image = global::BiblosDs.Document.AdminCentral.Properties.Resources.BindingNavigatorMovePreviousItem_Image1;
            this.radBtnPrev.Location = new System.Drawing.Point(71, 3);
            this.radBtnPrev.Name = "radBtnPrev";
            this.radBtnPrev.Size = new System.Drawing.Size(62, 25);
            this.radBtnPrev.TabIndex = 1;
            this.radBtnPrev.Text = "&Prev";
            this.radBtnPrev.Click += new System.EventHandler(this.radBtnPrev_Click);
            // 
            // radBtnFirst
            // 
            this.radBtnFirst.Image = global::BiblosDs.Document.AdminCentral.Properties.Resources.BindingNavigatorMoveFirstItem_Image1;
            this.radBtnFirst.Location = new System.Drawing.Point(3, 3);
            this.radBtnFirst.Name = "radBtnFirst";
            this.radBtnFirst.Size = new System.Drawing.Size(62, 25);
            this.radBtnFirst.TabIndex = 0;
            this.radBtnFirst.Text = "F&irst";
            this.radBtnFirst.Click += new System.EventHandler(this.radBtnFirst_Click);
            // 
            // UcPager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radRootPanel);
            this.Name = "UcPager";
            this.Size = new System.Drawing.Size(447, 31);
            ((System.ComponentModel.ISupportInitialize)(this.radRootPanel)).EndInit();
            this.radRootPanel.ResumeLayout(false);
            this.radRootPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnLast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLblPagesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSpinPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnPrev)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnFirst)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radRootPanel;
        private Telerik.WinControls.UI.RadButton radBtnLast;
        private Telerik.WinControls.UI.RadButton radBtnNext;
        private Telerik.WinControls.UI.RadLabel radLblPagesCount;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadSpinEditor radSpinPage;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton radBtnPrev;
        private Telerik.WinControls.UI.RadButton radBtnFirst;

    }
}
