using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using BiblosDs.Document.AdminCentral.ServiceReferenceLog;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.Configuration;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcLog : BaseAdminControls
    {
        private Guid selectedArchiveId;
        private Dictionary<int, Guid> archivesId;
        private static readonly string ERASING_CB_ITEM_TEXT = "NONE";
        private LogClient logClient = null;
        private const int TAKE = 20;

        public UcLog(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();
            this.selectedArchiveId = Guid.Empty;
            this.archivesId = new Dictionary<int, Guid>();
            this.radGridLogs.Columns["IdEntry"].DataType = typeof(Guid);
            //Fetch dati da DB + visualizzazione.
            this.Load += new EventHandler(UcLog_Load);
        }

        void UcLog_Load(object sender, EventArgs e)
        {
            base.CreateWaitDialog();
            var archives = Client.GetArchives();
            for (int i = 0; i < archives.Count(); i++)
            {
                this.archivesId.Add(i, archives[i].IdArchive);
                this.radCbArchives.Items.Add(new RadComboBoxItem(archives[i].Name));
            }
            radDtFrom.Value = DateTime.Now.Date;
            radDtTo.Value = DateTime.Now.Date;
            this.radCbArchives.Items.Add(new RadComboBoxItem(UcLog.ERASING_CB_ITEM_TEXT));
            base.CloseWaitDialog();

            try
            {
                if (logClient != null)
                {
                    try
                    {
                        logClient.GetLogsPagedCompleted -= logClient_GetLogsPagedCompleted;
                    }
                    catch { }
                }

                logClient = new LogClient("Binding_Log");
                logClient.GetLogsPagedCompleted += new EventHandler<GetLogsPagedCompletedEventArgs>(logClient_GetLogsPagedCompleted);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Enabled = false;
            }
        }

        private void radCheckDataTo_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            this.radDtTo.Enabled = this.radCheckDataTo.Checked;
        }

        private void radCheckIdArch_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            this.radCbArchives.Enabled = this.radCheckIdArch.Checked;
            this.selectedArchiveId = Guid.Empty;
            this.radCbArchives.Text = string.Empty;
        }

        private void radBtSearch_Click(object sender, EventArgs e)
        {
            this.ucPager.Initialize();
            this.searchLogs(0);
        }

        private void searchLogs(int pageNumber)
        {
            if (pageNumber < 0)
                pageNumber = 0;
            if (pageNumber > 0)
                pageNumber--;

            DateTime from = this.radDtFrom.Value, to = DateTime.Now.AddDays(1);
            if (this.radCheckDataTo.Checked)
            {
                DateTime tmp = this.radDtTo.Value;
                to = new DateTime(tmp.Year, tmp.Month, tmp.Day, 23, 59, 59, 999);
            }
            /*Imposta la data di partenza alla mezzanotte del giorno scelto.*/
            from = from.AddHours(-from.Hour);
            from = from.AddMinutes(-from.Minute);
            from = from.AddSeconds(-from.Second);
            /**/
            if (from >= to)
            {
                RadMessageBox.Show(this,
                    "Invalid dates (initial date is major or equal of final date).",
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
                return;
            }
            base.CreateWaitDialog();
            try
            {
                this.radGridLogs.Rows.Clear();
                Guid? idSelArchive = null;
                if (this.selectedArchiveId != Guid.Empty)
                    idSelArchive = this.selectedArchiveId;
                logClient.GetLogsPagedAsync(from, to, idSelArchive, pageNumber * TAKE, TAKE);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void logClient_GetLogsPagedCompleted(object sender, GetLogsPagedCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                if (e.Result.TotalRecords < 1)
                {
                    this.ucPager.Initialize();
                }
                else
                {
                    double maxPages = (double)e.Result.TotalRecords / (double)TAKE;

                    this.ucPager.Initialize(1, (int)Math.Ceiling(maxPages), this.ucPager.CurrentPage);
                }

                foreach (Log l in e.Result.Logs)
                {
                    this.radGridLogs.Rows.Add(new object[] 
                            {
                                l.IdEntry,
                                l.TimeStamp,
                                l.Message,
                                l.Server,
                                l.Client
                            });
                }
            }
            base.CloseWaitDialog();
        }

        private void radCbArchivesID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.radCbArchives.Text == UcLog.ERASING_CB_ITEM_TEXT)
            {
                this.selectedArchiveId = Guid.Empty;
            }
            else
            {
                this.selectedArchiveId = this.archivesId[this.radCbArchives.SelectedIndex];
            }
        }

        private void ucPager_ChangePage(object sender, int newPageNumber)
        {
            this.searchLogs(newPageNumber);
        }
    }
}
