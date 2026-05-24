using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IBarcodeService
    {
        public string GenerateBarcodeImage(string barcode, string webRootPath);
        public string GenerateEAN13();
    }
}
