using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Amount { get; set; }
        public int AmountAvailable { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
    }
}