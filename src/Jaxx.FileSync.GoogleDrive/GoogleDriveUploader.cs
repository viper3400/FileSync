using System;
using System.Collections.Generic;
using System.Linq;
using Jaxx.FileSync.Shared.Interfaces;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace Jaxx.FileSync.GoogleDrive
{
    public class GoogleDriveUploader : IUploader
    {
        List<string> _grantedUsers;
        DriveService _service;


        public GoogleDriveUploader(IEnumerable<string> grantedUsers, IGoogleAccountProvider accountProvider)
        {
            _grantedUsers = grantedUsers.ToList();
            _service = accountProvider.CreateDriveService();
        }

        public bool UploadFile(string uploadFile, string uploadFolder)
        {
            File mySqlFolder;
            var mySqlFolderSearch = DriveApi.GetFiles(_service, $"name='{uploadFolder}'");
            if (mySqlFolderSearch.Count == 1)
            {
                mySqlFolder = mySqlFolderSearch.FirstOrDefault();
            }
            else
            {
                throw new ArgumentException("Folder not found.", "uploadFolder");
            }

            var file = DriveApi.UploadFile(_service, uploadFile, mySqlFolder.Id);
            foreach (var user in _grantedUsers)
            {
                DriveApi.InsertPermission(_service, file.Id, user, "user", "writer");
            }

            return true;
        }

        
    }
}
