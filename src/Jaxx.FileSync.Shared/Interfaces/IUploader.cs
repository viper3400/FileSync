using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jaxx.FileSync.Shared.Interfaces
{
    public interface IUploader
    {
        bool UploadFile(string uploadFile, string uploadFolder);
    }
}
