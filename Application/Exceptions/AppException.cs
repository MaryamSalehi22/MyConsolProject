using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }

        public int? ErrorCode { get; set; }
    }
}
