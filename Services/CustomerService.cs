using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Desktop.Models;

namespace HotelManagement.Desktop.Services
{
    public class CustomerService
    {
        // Danh sách tĩnh lưu trữ tạm trong bộ nhớ để giả lập cơ sở dữ liệu
        private static readonly List<CustomerDto> _customers = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = 1, FullName = "Nguyễn Văn An", PhoneNumber = "0901234567", Email = "an.nguyen@gmail.com", IdentityCard = "012345678910" },
            new CustomerDto { CustomerId = 2, FullName = "Trần Thị Bình", PhoneNumber = "0918765432", Email = "binh.tran@gmail.com", IdentityCard = "098765432101" }
        };

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            await Task.Delay(100);
            return _customers.ToList();
        }

        public async Task SaveCustomerAsync(CustomerDto customer)
        {
            await Task.Delay(100);
            if (customer.CustomerId == 0)
            {
                // Thêm mới
                customer.CustomerId = _customers.Count > 0 ? _customers.Max(c => c.CustomerId) + 1 : 1;
                _customers.Add(customer);
            }
            else
            {
                // Cập nhật thông tin khách hàng hiện có
                var existing = _customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
                if (existing != null)
                {
                    existing.FullName = customer.FullName;
                    existing.PhoneNumber = customer.PhoneNumber;
                    existing.Email = customer.Email;
                    existing.IdentityCard = customer.IdentityCard;
                }
            }
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            await Task.Delay(100);
            var customer = _customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
        }
    }
}