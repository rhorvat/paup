using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class OfferViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public List<GameViewModel> Games { get; set;}
        public DateTime ShippingDate { get; set; }
        public int OrderId { get; set; }
        public string Guarantee { get; set; }
        public decimal Rabat { get; set; }
    }
}