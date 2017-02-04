using Google.Apis.Drive.v3;
using Jaxx.FileSync.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaxx.FileSync.GoogleDrive
{
    public class GoogleDriveFolderController : IFolderController
    {
        List<string> _grantedUsers;
        DriveService _service;


        public GoogleDriveFolderController(IEnumerable<string> grantedUsers, IGoogleAccountProvider accountProvider)
        {
            _grantedUsers = grantedUsers.ToList();
            _service = accountProvider.CreateDriveService();
        }

        public bool CreateFolder(string name, string parentFolder)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFolder(string name)
        {
            throw new NotImplementedException();
        }
    }
}
