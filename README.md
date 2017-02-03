# FileSync

FileSync is very basic implementation of .Net GoogleDriveAPI v3. It Uploads a single file to Google Drive cloud storage, using a Google [service account](https://developers.google.com/api-client-library/dotnet/get_started#service-accounts). It grants permission to this file to another Google Drive account.

## Usage

```
Jaxx.FileSync.exe 
      -c ["PATH_TO_CERTFILE\key.p12"] 
      -s ["YOUR_SERVICE_ACCOUNT@gserviceaccount.com"] 
      create file
      -g ["DRIVEUSER@googlemail.com"] 
      -f ["UPLOAD_FILE"]
      -d ["root"]
```
