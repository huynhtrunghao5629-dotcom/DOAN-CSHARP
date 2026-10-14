using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Desktop.Models;

namespace HotelManagement.Desktop.Services
{
    public class InvoiceService
    {
        private static readonly List<InvoiceDto> _invoices = new List<InvoiceDto>
        {
            new InvoiceDto
            {
                InvoiceId = 1,
                BookingId = 1,
                CustomerName = "Nguyễn Văn An",
                RoomNumber = "101",
                TotalAmount = 600000,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Tiền mặt",
                Status = "Đã thanh toán"
            }
        };

        public async Task<List<InvoiceDto>> GetAllInvoicesAsync()
        {
            await Task.Delay(100);
            return _invoices.ToList();
        }

        public async Task SaveInvoiceAsync(InvoiceDto invoice)
        {
            await Task.Delay(100);
            if (invoice.InvoiceId == 0)
            {
                invoice.InvoiceId = _invoices.Count > 0 ? _invoices.Max(i => i.InvoiceId) + 1 : 1;
                _invoices.Add(invoice);
            }
            else
            {
                var existing = _invoices.FirstOrDefault(i => i.InvoiceId == invoice.InvoiceId);
                if (existing != null)
                {
                    existing.BookingId = invoice.BookingId;
                    existing.CustomerName = invoice.CustomerName;
                    existing.RoomNumber = invoice.RoomNumber;
                    existing.TotalAmount = invoice.TotalAmount;
                    existing.PaymentDate = invoice.PaymentDate;
                    existing.PaymentMethod = invoice.PaymentMethod;
                    existing.Status = invoice.Status;
                }
            }
        }

        public async Task DeleteInvoiceAsync(int invoiceId)
        {
            await Task.Delay(100);
            var invoice = _invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
            if (invoice != null)
            {
                _invoices.Remove(invoice);
            }
        }
    }
}