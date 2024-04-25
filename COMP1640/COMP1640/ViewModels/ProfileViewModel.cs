namespace COMP1640.ViewModels
{
    public class ProfileViewModel
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
