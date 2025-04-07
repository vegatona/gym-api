using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mockup.MVVM.Models
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string nip { get; set; }
        public string clave_Seguridad { get; set; }
    }
}
