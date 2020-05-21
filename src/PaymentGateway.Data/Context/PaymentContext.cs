using Microsoft.EntityFrameworkCore;
using PaymentGateway.Model.PaymentRepository;

namespace PaymentGateway.Data.Context
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
        }

        public DbSet<PaymentRecord> Payments { get; set; }
    }
}
