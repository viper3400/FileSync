using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Jaxx.FileSync.GoogleDrive
{
    public class GoogleCertServiceAccountProvider : IGoogleAccountProvider
    {
        string _certFile;
        string _serviceAccountEmail;

        public GoogleCertServiceAccountProvider(string certFile, string serviceAccountEmail)
        {
            _certFile = certFile;
            _serviceAccountEmail = serviceAccountEmail;
        }

        public DriveService CreateDriveService()
        {
            var certificate = new X509Certificate2(_certFile, "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(_serviceAccountEmail)
               {
                   Scopes = new[] { DriveService.Scope.DriveFile, DriveService.Scope.Drive }
               }.FromCertificate(certificate));

            // Create the service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "JaxxDriveSync",
            });

            return service;
        }
    }
}
