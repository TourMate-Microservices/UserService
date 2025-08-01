﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
using TourMate.UserService.Repositories.ResponseModels;

namespace TourMate.UserService.Services.IServices
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByAccId(int accId);
        Task<bool> CreateCustomer(Customer customer);
        Task<Customer> GetCustomerByPhone(string phone);
        Task<PagedResult<Customer>> GetPagedCustomersAsync(int pageIndex, int pageSize, string fullName);
        Task<Customer> GetCustomerById(int id);
        Task<bool> UpdateCustomer(int customerId, CustomerUpdateRequest request);
    }
}
