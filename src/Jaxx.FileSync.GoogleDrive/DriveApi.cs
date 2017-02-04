using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Jaxx.FileSync.GoogleDrive
{
    public static class DriveApi
    {
        /// Create a new Directory.
        /// Documentation: https://developers.google.com/drive/v2/reference/files/insert
        /// 

        /// a Valid authenticated DriveService
        /// The title of the file. Used to identify file or folder name.
        /// A short description of the file.
        /// Collection of parent folders which contain this file. 
        ///                       Setting this field will put the file in all of the provided folders. root folder.
        /// 
        public static File createDirectory(DriveService _service, string _title, string _description, string _parent)
        {

            File NewDirectory = null;


            // Init the parentId
            string parentId = "root";
            // Get the file id of the parent Folder, in case it is not "root"
            if (_parent.ToUpper() != "ROOT")
            {
                var parentList = GetFilesByName(_service, _parent, NameSearchOperators.Is);
                // it's possible, that folder name was given more than once -> for this time we will handle the 
                // request just in case we find a unique folder name, otherwise we are not sure, which folder we found.
                if (parentList.Count() != 1)
                {
                    throw new ArgumentException($"Parent {_parent} not found", "parent");
                }

                parentId = parentList.FirstOrDefault().Id;
            }
            // in case parentId is still null, we will create our new folder in root folder
            

            // Create metaData for a new Directory
            File body = new File();
            body.Name = _title;
            body.Description = _description;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List<string> { parentId };
            try
            {
                FilesResource.CreateRequest request = _service.Files.Create(body);
                NewDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return NewDirectory;
        }

        /// <summary>
        /// Insert a new permission.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <param name="fileId">ID of the file to insert permission for.</param>
        /// <param name="who">
        /// User or group e-mail address, domain name or null for "default" type.
        /// </param>
        /// <param name="type">The value "user", "group", "domain" or "default".</param>
        /// <param name="role">The value "owner", "writer" or "reader".</param>
        /// <returns>The inserted permission, null is returned if an API error occurred</returns>
        public static Permission InsertPermission(DriveService service, String fileId, String who, String type, String role)
        {
            Permission newPermission = new Permission();
            newPermission.EmailAddress = who;
            newPermission.Type = type;
            newPermission.Role = role;
            try
            {
                var request = service.Permissions.Create(newPermission, fileId);
                request.SendNotificationEmail = false;
                return request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return null;
        }

        /// 

        /// Uploads a file
        /// Documentation: https://developers.google.com/drive/v2/reference/files/insert
        /// 

        /// a Valid authenticated DriveService
        /// path to the file to upload
        /// Collection of parent folders which contain this file. 
        ///                       Setting this field will put the file in all of the provided folders. root folder.
        /// If upload succeeded returns the File resource of the uploaded file 
        ///          If the upload fails returns null
        public static File UploadFile(DriveService _service, string _uploadFile, string _parent)
        {

            if (System.IO.File.Exists(_uploadFile))
            {
                File body = new File();
                body.Name = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "File uploaded by Automated MySqlBackup";
                body.MimeType = GetMimeType(_uploadFile);
                body.Parents = new List<string> { _parent };
                body.WritersCanShare = true;

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, GetMimeType(_uploadFile));
                    request.Upload();

                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("File does not exist: " + _uploadFile);
                return null;
            }

        }

        /// List all of the files and directories for the current user.  
        /// 
        /// Documentation: https://developers.google.com/drive/v2/reference/files/list
        /// Documentation Search: https://developers.google.com/drive/web/search-parameters
        /// 
        ///a Valid authenticated DriveService        
        ///if Search is null will return all files        
        /// 
        public static List<File> GetFiles(DriveService service, string search)
        {

            var Files = new List<File>();

            try
            {
                //List all of the files and directories for the current user.  
                // Documentation: https://developers.google.com/drive/v2/reference/files/list
                FilesResource.ListRequest list = service.Files.List();
                if (search != null)
                {
                    list.Q = search;
                }
                FileList filesFeed = list.Execute();

                //// Loop through until we arrive at an empty page
                while (filesFeed.Files != null)
                {
                    // Adding each item  to the list.
                    foreach (File item in filesFeed.Files)
                    {
                        Files.Add(item);
                    }

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }

                    // Prepare the next page of results
                    list.PageToken = filesFeed.NextPageToken;

                    // Execute and process the next page request
                    filesFeed = list.Execute();
                }
            }
            catch (Exception ex)
            {
                // In the event there is an error with the request.
                Console.WriteLine(ex.Message);
            }
            return Files;
        }

        /// <summary>
        /// List all of the files and directories matching the search name
        /// </summary>
        /// <param name="service"></param>
        /// <param name="name">The name to search for.</param>
        /// <param name="searchOperator">Choose an operator out of 'contains' (default), '=', '!='</param>
        /// <returns></returns>
        public static List<File> GetFilesByName(DriveService service, string name, NameSearchOperators searchOperator)
        {
            switch(searchOperator)
            {
                default:
                case NameSearchOperators.Contains:                
                    return GetFiles(service, $"name contains '{name}'");
                case NameSearchOperators.Is:
                    return GetFiles(service, $"name = '{name}'");
                case NameSearchOperators.IsNot:
                    return GetFiles(service, $"name != '{name}'");   
            }
        }

        // tries to figure out the mime type of the file.
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            //TODO: Eliminate Win32 dependency
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        /// <summary>
        /// Operators for GetFileNameSearch
        /// </summary>
        public enum NameSearchOperators { Contains, Is, IsNot };
    }
}

