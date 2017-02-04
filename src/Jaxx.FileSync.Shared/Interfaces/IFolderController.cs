using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jaxx.FileSync.Shared.Interfaces
{
    interface IFolderController
    {
        bool CreateFolder(string name, string parentFolder);
        bool DeleteFolder(string name);
    }
}
