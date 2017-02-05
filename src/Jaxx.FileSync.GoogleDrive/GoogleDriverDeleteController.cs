using Google.Apis.Drive.v3;
using Jaxx.FileSync.Shared.Interfaces;
using System;
using System.Globalization;

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
            var time = DateTime.Now - new TimeSpan(fileAgeInDays, 0, 0, 0, 0);
            
            string timeString = time.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            //var files = DriveApi.GetFiles(_service, $"modifiedTime <= '{timeString}' and '{folder}' in parents");
            var files = DriveApi.GetFiles(_service, $"modifiedTime <= '{timeString}'");

            foreach (var file in files)
            {
                Console.WriteLine($"This would delete {file.Name}, last modified on {file.ModifiedTime.Value}");                
            }

            return true;
        }
    }
}
