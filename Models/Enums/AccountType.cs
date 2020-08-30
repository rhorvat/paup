using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public enum AccountType
    {
        Admin = 1,
        Korisnik = 2
    }
}