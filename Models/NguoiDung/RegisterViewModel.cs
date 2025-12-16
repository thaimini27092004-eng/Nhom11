namespace WebsiteBanHang.Models.NguoiDung
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string SDT { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string HoTen { get; set; }

        [Display(Name = "Address")]
        public string DiaChi { get; set; }

        [Display(Name = "Age")]
        public DateTime? NgaySinh { get; set; }
        [Display(Name = "Avatar")]
        public IFormFile? AvatarFile { get; set; } // Thêm trường để upload file ảnh

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
