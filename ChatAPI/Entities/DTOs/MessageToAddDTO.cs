using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class MessageToAddDTO
    {
        public string Text { get; set; }
        public Guid SenderId { get; set; }
    }
}
