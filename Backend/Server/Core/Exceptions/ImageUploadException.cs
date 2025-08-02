using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exceptions
{
    public class ImageUploadException : Exception
    {
        public ImageUploadException(string message) : base(message) { }
        public ImageUploadException(string message, Exception innerException) : base(message, innerException) { }
    }

}
