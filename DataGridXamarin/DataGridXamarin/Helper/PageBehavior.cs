using System;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.SfDataGrid.XForms.Exporting;
using Xamarin.Forms;

namespace DataGridXamarin
{
    public class PageBehavior : Behavior<ContentPage>
    {
        #region Fields

        private SfDataGrid sfDataGrid1 = null;
        private SfDataGrid sfDataGrid2 = null;
        private Button button = null;
        #endregion

        #region Overrides

        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            button = bindable.FindByName<Button>("exportPdf");
            sfDataGrid1 = bindable.FindByName<SfDataGrid>("sfGrid");
            sfDataGrid2 = bindable.FindByName<SfDataGrid>("sfGrid1");
            button.Clicked += Button_Clicked;
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            base.OnDetachingFrom(bindable);
            button.Clicked -= Button_Clicked;
            button = null;
            sfDataGrid1 = null;
            sfDataGrid2 = null;
        }
        
        private void Button_Clicked(object sender, EventArgs e)
        {
            // First Grid
            DataGridPdfExportingController pdfExport = new DataGridPdfExportingController();
            MemoryStream stream = new MemoryStream();
            var exportToPdf = pdfExport.ExportToPdf(this.sfDataGrid1, new DataGridPdfExportOption()
            {
                FitAllColumnsInOnePage = true,
            });
            exportToPdf.Save(stream);
            exportToPdf.Close(true);

            // Second Grid 
            DataGridPdfExportingController pdfExport1 = new DataGridPdfExportingController();
            MemoryStream stream1 = new MemoryStream();
            var exportToPdf1 = pdfExport1.ExportToPdf(this.sfDataGrid2, new DataGridPdfExportOption()
            {
                FitAllColumnsInOnePage = true,
            });
            exportToPdf1.Save(stream1);
            exportToPdf1.Close(true);

            // Final PDF 
            PdfDocument document = new PdfDocument();
            MemoryStream laststream = new MemoryStream();
            Stream[] source = { stream, stream1 };
            PdfDocumentBase.Merge(document, source);
            document.Save(laststream);

            if (Device.RuntimePlatform == Device.UWP)
                Xamarin.Forms.DependencyService.Get<ISaveWindows>().Save("DataGrid.pdf", "application/pdf", laststream);
            else
                Xamarin.Forms.DependencyService.Get<ISave>().Save("DataGrid.pdf", "application/pdf", laststream);
        }



        #endregion
    }
}
