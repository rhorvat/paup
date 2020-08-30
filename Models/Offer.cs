using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class Offer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string Guarantee { get; set; }
        public DateTime ShippingDate { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}