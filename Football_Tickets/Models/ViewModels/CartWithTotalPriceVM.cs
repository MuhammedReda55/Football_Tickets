
namespace Football_Tickets.Models.ViewModels
{
    public class CartWithTotalPriceVM
    {
        public List<Cart> Carts { get; set; }
        public string? UserName { get; set; }
        public double TotalPrice { get; set; }
        public string? SelectedSection { get; set; }
    }
}
