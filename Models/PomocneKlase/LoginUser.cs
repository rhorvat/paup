using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}