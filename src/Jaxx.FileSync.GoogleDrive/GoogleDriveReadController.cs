using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jaxx.FileSync.Shared.Interfaces;
using Google.Apis.Drive.v3;

namespace Jaxx.FileSync.GoogleDrive
{
    public class GoogleDriveReadController : IReadController
    {
        DriveService _service;

        public GoogleDriveReadController(IGoogleAccountProvider accountProvider)
        {
            _service = accountProvider.CreateDriveService();
        }

        public IEnumerable<string> List()
        {
            var fileList = DriveApi.GetFiles(_service, null);
            var stringList = new List<string>();

            foreach(var file in fileList)
            {
                var fileString = $"[{file.MimeType}] -> {file.Name}, {file.ModifiedTime}";
                stringList.Add(fileString);
            }

            return stringList;
        }
    }
}
