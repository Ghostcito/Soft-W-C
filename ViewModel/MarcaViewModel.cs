using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models;

namespace SoftWC.ViewModel
{
    public class MarcaViewModel
    {
        public Usuario usuario { get; set; }
        public (Sede, bool) verificacion { get; set; }
    }
}