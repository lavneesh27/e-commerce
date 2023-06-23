namespace WebApplication1.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public User User { get; set; } = new User();
        public PaymentMethod PaymentMethod { get; set; } = new PaymentMethod();
        public int TotalAmount { get; set; }
        public int ShippingCharges { get; set; }
        public int AmountReduced { get; set; }
        public int AmountPaid { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
