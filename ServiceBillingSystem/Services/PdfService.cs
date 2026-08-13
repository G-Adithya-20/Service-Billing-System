using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceBillingSystem.Models;

namespace ServiceBillingSystem.Services;

public class PdfService
{
    public byte[] GenerateInvoice(Bill bill, Company company)
    {
        var document = Document.Create(container => //creates pdf
        {
            container.Page(page => //create a page
            {
                page.Size(PageSizes.A4); //page size
                page.Margin(40); //space around page
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                  column.Item().Row(row => => //Column means items are placed one below another
                    {
                        // Company info
                        row.RelativeItem().Column(companyColumn 
                        {
                            //Item() = one element inside that row/column
                            companyColumn.Item().Text(company.name).Bold().FontSize(20);
                            companyColumn.Item().Text(company.address).FontSize(10).FontColor(Colors.Grey.Darken1);
                            companyColumn.Item().Text("Phone: " + company.phonenumber).FontSize(9).FontColor(Colors.Grey.Darken1);
                            companyColumn.Item().Text("Email: " + company.email).FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        // Invoice info
                        row.ConstantItem(180).AlignRight().Column(invoice => //Row means items are placed side by side
                        {
                            invoice.Item().Text("INVOICE").Bold().FontSize(24);
                            invoice.Item().Text($"#{bill.BillNumber}").FontSize(10);
                        });
                      });

                    column.Item().PaddingTop(15).LineHorizontal(1);
                });

                page.Content().PaddingTop(20).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text("BILL TO").Bold().FontSize(9).FontColor(Colors.Grey.Darken1);
                            left.Item().PaddingTop(4).Text(bill.Customer?.Name ?? "Customer").Bold().FontSize(12);
                            left.Item().Text(bill.Customer?.Phone ?? "");
                            left.Item().Text(bill.Customer?.Email ?? "");
                            left.Item().Text(bill.Customer?.Address ?? "");
                        });

                        row.ConstantItem(180).Column(right =>
                        {
                            right.Item().Text("INVOICE DETAILS").Bold().FontSize(9).FontColor(Colors.Grey.Darken1);

                            right.Item().PaddingTop(5).Row(r =>
                            {
                                r.RelativeItem().Text("Invoice Date");
                                r.RelativeItem().AlignRight().Text(bill.BillDate.ToString("dd-MM-yyyy"));
                            });

                            right.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Staff");
                                r.RelativeItem().AlignRight().Text(bill.Staff?.Name ?? "-");
                            });

                            right.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Payment");
                                r.RelativeItem().AlignRight().Text(bill.PaymentStatus).Bold();
                            });
                        });
                    });

                    column.Item().PaddingTop(25);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(35);
                            columns.RelativeColumn(4);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(90);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8).Text("#").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8).Text("SERVICE").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8).AlignRight().Text("QTY").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8).AlignRight().Text("UNIT PRICE").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Grey.Darken3).Padding(8).AlignRight().Text("TOTAL").Bold().FontColor(Colors.White);
                        });

                        int number = 1;

                        foreach (var item in bill.BillItems)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(number.ToString());

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(item.Service?.Name ?? "Service");

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight().Text(item.Quantity.ToString());

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight().Text($"₹{item.UnitPrice:N2}");

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight().Text($"₹{item.Total:N2}");

                            number++;
                        }
                    });

                    column.Item().PaddingTop(20).Row(row =>
                    {
                        // Left side note
                        row.RelativeItem().Column(note =>
                        {
                            note.Item().Text("Thank you for your business!").Bold().FontSize(11);

                            note.Item().PaddingTop(5).Text("Please keep this invoice " + "for your records.").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        // Right totals box
                        row.ConstantItem(230).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(12).Column(total =>
                            {
                                total.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Subtotal");
                                    r.ConstantItem(90).AlignRight().Text($"₹{bill.SubTotal:N2}");
                                });

                                total.Item().PaddingTop(6).Row(r =>
                                {
                                    r.RelativeItem().Text("Discount");
                                    r.ConstantItem(90).AlignRight().Text($"- ₹{bill.Discount:N2}");
                                });

                                total.Item().PaddingTop(6).Row(r =>
                                {
                                    r.RelativeItem().Text("GST");
                                    r.ConstantItem(90).AlignRight().Text($"₹{bill.Tax:N2}");
                                });

                                total.Item().PaddingTop(10).LineHorizontal(1);

                                total.Item().PaddingTop(10).Row(r =>
                                {
                                    r.RelativeItem().Text("GRAND TOTAL").Bold().FontSize(12);
                                    r.ConstantItem(90).AlignRight().Text($"₹{bill.GrandTotal:N2}").Bold().FontSize(12);
                                });
                            });
                    });

                    column.Item().PaddingTop(20).AlignRight().Text($"Payment Status: {bill.PaymentStatus}").Bold().FontSize(10);
                });

                page.Footer().AlignCenter().Column(column =>
                {
                    column.Item().LineHorizontal(1);

                    column.Item().PaddingTop(8).Text("Service Billing System • " + "Thank you for choosing us!").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        });

        return document.GeneratePdf();
    }
}