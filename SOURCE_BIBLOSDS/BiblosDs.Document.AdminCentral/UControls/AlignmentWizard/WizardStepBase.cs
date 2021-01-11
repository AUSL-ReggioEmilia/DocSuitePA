using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    public class WizardStepBase : UserControl
    {
        private Panel pnlStepControls;
        private FlowLayoutPanel flowSteps;
        private Button btnPrevStep;
        private Panel pnlTitle;
        private Label lblTitle;
        protected Panel pnlContent;
        private Button btnNextStep;

        /// <summary>
        /// 
        /// </summary>
        private sealed class EmptyWizardStep : WizardStepBase
        {
            public EmptyWizardStep() : base() { this.Initialize(string.Empty, null, null); }
        }

        protected WizardStepBase PrevStep { get; private set; }
        protected WizardStepBase NextStep { get; private set; }
        protected Control ParentControl { get; private set; }
        protected Panel ContentPanel { get { return this.pnlContent; } }

        public string Title
        {
            get { return this.lblTitle.Text; }
            private set { this.lblTitle.Text = value; }
        }

        protected WizardStepBase()
        {
            this.InitializeComponent();
        }

        public static WizardStepBase CreateEmptyWizardStep()
        {
            return new EmptyWizardStep();
        }

        public virtual void Initialize(string title, WizardStepBase previousStep, WizardStepBase nextStep)
        {
            this.Title = title;

            this.PrevStep = previousStep;
            this.NextStep = nextStep;

            btnNextStep.Enabled = (this.NextStep != null);
            btnPrevStep.Enabled = (this.PrevStep != null);
        }

        public virtual void Show(Control parentControl)
        {
            this.ParentControl = parentControl;
        }

        protected void Show(Control parentControl, WizardStepBase controlBeingShown)
        {
            if (controlBeingShown != null)
            {
                this.ParentControl = parentControl;
                if (this.ParentControl != null)
                {
                    var controls = this.ParentControl.Controls;

                    Control newActiveControl = null;

                    for (int i = 0; i < this.ParentControl.Controls.Count; i++)
                    {
                        var ctl = this.ParentControl.Controls[i];

                        try { ctl.Hide(); }
                        catch { }

                        if (ctl.GetType() == typeof(WizardStepBase) && (ctl as WizardStepBase).Title == controlBeingShown.Title)
                        {
                            newActiveControl = ctl;
                        }
                    }

                    if (newActiveControl == null)
                    {
                        try { controlBeingShown.Dock = DockStyle.Fill; }
                        catch { }
                        this.ParentControl.Controls.Add(controlBeingShown);
                    }

                    controlBeingShown.Show();

                    Application.DoEvents();
                }
            }
        }

        public virtual void GotoNextStep()
        {
            if (this.NextStep != null)
            {
                this.Hide();
                this.NextStep.Show(this.ParentControl);
                Application.DoEvents();
            }
        }

        public virtual void GotoPreviousStep()
        {
            if (this.PrevStep != null)
            {
                this.Hide();
                this.PrevStep.Show(this.ParentControl);
                Application.DoEvents();
            }
        }

        private void InitializeComponent()
        {
            this.pnlStepControls = new System.Windows.Forms.Panel();
            this.flowSteps = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrevStep = new System.Windows.Forms.Button();
            this.btnNextStep = new System.Windows.Forms.Button();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlStepControls.SuspendLayout();
            this.flowSteps.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlStepControls
            // 
            this.pnlStepControls.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStepControls.Controls.Add(this.flowSteps);
            this.pnlStepControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStepControls.Location = new System.Drawing.Point(0, 429);
            this.pnlStepControls.Name = "pnlStepControls";
            this.pnlStepControls.Size = new System.Drawing.Size(636, 47);
            this.pnlStepControls.TabIndex = 0;
            // 
            // flowSteps
            // 
            this.flowSteps.Controls.Add(this.btnPrevStep);
            this.flowSteps.Controls.Add(this.btnNextStep);
            this.flowSteps.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowSteps.Location = new System.Drawing.Point(426, 0);
            this.flowSteps.Name = "flowSteps";
            this.flowSteps.Size = new System.Drawing.Size(206, 43);
            this.flowSteps.TabIndex = 0;
            // 
            // btnPrevStep
            // 
            this.btnPrevStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrevStep.AutoSize = true;
            this.btnPrevStep.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevStep.Location = new System.Drawing.Point(3, 3);
            this.btnPrevStep.Name = "btnPrevStep";
            this.btnPrevStep.Size = new System.Drawing.Size(104, 40);
            this.btnPrevStep.TabIndex = 1;
            this.btnPrevStep.Text = "<- Previous";
            this.btnPrevStep.UseVisualStyleBackColor = true;
            this.btnPrevStep.Click += new System.EventHandler(this.btnPrevStep_Click);
            // 
            // btnNextStep
            // 
            this.btnNextStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextStep.AutoSize = true;
            this.btnNextStep.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNextStep.Location = new System.Drawing.Point(113, 3);
            this.btnNextStep.Name = "btnNextStep";
            this.btnNextStep.Size = new System.Drawing.Size(90, 40);
            this.btnNextStep.TabIndex = 0;
            this.btnNextStep.Text = "Next ->";
            this.btnNextStep.UseVisualStyleBackColor = true;
            this.btnNextStep.Click += new System.EventHandler(this.btnNextStep_Click);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(636, 64);
            this.pnlTitle.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(636, 64);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "TITLE";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlContent
            // 
            this.pnlContent.AutoScroll = true;
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 64);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(636, 365);
            this.pnlContent.TabIndex = 2;
            // 
            // WizardStepBase
            // 
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.pnlStepControls);
            this.Name = "WizardStepBase";
            this.Size = new System.Drawing.Size(636, 476);
            this.pnlStepControls.ResumeLayout(false);
            this.flowSteps.ResumeLayout(false);
            this.flowSteps.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void pnlStepControls_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            this.GotoNextStep();
        }

        private void btnPrevStep_Click(object sender, EventArgs e)
        {
            this.GotoPreviousStep();
        }
    }

}
