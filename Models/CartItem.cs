namespace SurgiTech.Models
{
    // Lives only in session (JSON), not in the database.
    // Kept intentionally simple to match the existing Order model,
    // which stores a single TotalAmount rather than line items.
    public class CartItem
    {
        public int InstrumentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;
    }
}