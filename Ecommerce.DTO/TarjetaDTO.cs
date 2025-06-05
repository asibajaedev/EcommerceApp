using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO
{
    public class TarjetaDTO
    {
        [Required(ErrorMessage = "Ingrese titular")]
        public string? Titular { get; set; }
        [Required(ErrorMessage = "Ingrese numero tarjeta")]
        public string? NumeroTarjeta { get; set; }
        [Required(ErrorMessage = "Ingrese Vigencia")]
        public string? FechaVigencia { get; set; }
        [Required(ErrorMessage = "Ingrese CVV")]
        public string? CVV { get; set; }
    }
}
