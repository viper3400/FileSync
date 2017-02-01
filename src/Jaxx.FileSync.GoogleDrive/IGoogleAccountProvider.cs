using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jaxx.FileSync.GoogleDrive
{
    public interface IGoogleAccountProvider
    {
        DriveService CreateDriveService();
    }
}
