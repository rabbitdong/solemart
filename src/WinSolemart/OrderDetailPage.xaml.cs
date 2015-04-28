using Solemart.DataProvider.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinSolemart.Models;

namespace WinSolemart
{
    /// <summary>
    /// Interaction logic for OrderDetail.xaml
    /// </summary>
    public partial class OrderDetailPage : Page
    {
        public OrderDetailPage(OrderListViewModel model)
        {
            InitializeComponent();

            TableRowGroup rowGroup = new TableRowGroup();
            foreach (OrderDetailItem item in model.OrderDetails)
            {
                TableRow tr = new TableRow();
                TableCell tcProductName = new TableCell(new Paragraph(new Run(item.Product.ProductName)));
                TableCell tcAmount = new TableCell(new Paragraph(new Run(item.Amount.ToString())));
                TableCell tcUnitPrice = new TableCell(new Paragraph(new Run(item.UnitPrice.ToString())));
                TableCell tcTotalAmount = new TableCell(new Paragraph(new Run((item.UnitPrice * item.Amount).ToString())));
                tr.Cells.Add(tcProductName);
                tr.Cells.Add(tcAmount);
                tr.Cells.Add(tcUnitPrice);
                tr.Cells.Add(tcTotalAmount);
                rowGroup.Rows.Add(tr);
            }

            tblOrderDetail.RowGroups.Add(rowGroup);
        }

        /// <summary>
        /// Print the flowdocument
        /// </summary>
        /// <param name="document"></param>
        private void DoThePrint(FlowDocument document)
        {
            // Clone the source document's content into a new FlowDocument.
            // This is because the pagination for the printer needs to be
            // done differently than the pagination for the displayed page.
            // We print the copy, rather that the original FlowDocument.
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
            source.Save(s, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(s, DataFormats.Xaml);

            // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
            // and allowing the user to select a printer.

            // get information about the dimensions of the seleted printer+media.
            PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness t = new Thickness(72);  // copy.PagePadding;
                copy.PagePadding = new Thickness(
                                 Math.Max(ia.OriginWidth, t.Left),
                                   Math.Max(ia.OriginHeight, t.Top),
                                   Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
                                   Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

                copy.ColumnWidth = double.PositiveInfinity;
                //copy.PageWidth = 528; // allow the page to be the natural with of the output device

                // Send content to the printer.
                docWriter.Write(paginator);
            }

        }
    }
}
