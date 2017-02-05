using Google.Apis.Drive.v3;
using Jaxx.FileSync.Shared.Interfaces;
using System;

namespace Jaxx.FileSync.GoogleDrive
{
    public class GoogleDriverDeleteController : IDeleteController
    {
        DriveService _service;

        public GoogleDriverDeleteController(IGoogleAccountProvider accountProvider)
        {            
            _service = accountProvider.CreateDriveService();
        }

        public bool DeleteAgedFiles(int fileAgeInDays, string folder)
        {
            throw new NotImplementedException();
        }
    }
}
