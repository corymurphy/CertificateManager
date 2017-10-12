using System;
using System.ComponentModel.DataAnnotations;

namespace CertificateManager.Entities
{
    public class LoginLocalViewModel
    {
        [Required]
        public string UserPrincipalName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public Guid Domain { get; set; }
    }
}
