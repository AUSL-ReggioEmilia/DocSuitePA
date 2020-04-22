using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BiblosDS.UI.ConsoleTest.UControls
{
    public partial class UcDocumentViewer : Form
    {
        public UcDocumentViewer()
        {
            InitializeComponent();
        }

        public UcDocumentViewer(byte[] blob)
        {
            InitializeComponent();            
            webBrowser1.DocumentStream = new MemoryStream(blob);
        }

        public UcDocumentViewer(string text)
        {
            InitializeComponent();
            webBrowser1.DocumentStream = new MemoryStream(Encoding.ASCII.GetBytes(text));            
        }       
    }
}
