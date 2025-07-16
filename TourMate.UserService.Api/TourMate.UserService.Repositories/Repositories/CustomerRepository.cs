using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Context;
using TourMate.UserService.Repositories.IRepositories;

namespace TourMate.UserService.Repositories.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private TourMateUserContext _context;

        public CustomerRepository()
        {
            _context ??= new();
        }

        public CustomerRepository(TourMateUserContext context)
        {
            _context = context;
        }


    }
}
