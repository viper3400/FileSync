using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jaxx.FileSync.Shared.Interfaces
{
    public interface IDeleteController
    {
        bool DeleteAgedFiles(int fileAgeInDays, string folder, bool preview);
        bool DeleteObject(string objectId);
    }
}
