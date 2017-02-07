using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jaxx.FileSync.Shared.Interfaces
{
    public interface IReadController
    {
        IEnumerable<string> List();
    }
}
