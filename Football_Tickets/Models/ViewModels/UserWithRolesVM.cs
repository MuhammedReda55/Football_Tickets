namespace Football_Tickets.Models.ViewModels
{
    public class UserWithRolesVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public List<string> Roles { get; set; }
        public bool IsBlocked { get; set; }
    }
}
