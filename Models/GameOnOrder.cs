using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class GameOnOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Game Game{ get; set; }
        public int GameId { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public int Amount { get; set; }
    }
}