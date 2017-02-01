# FileSync
* Very basic implementation of .Net GoogleDriveAPI v3.
* Uploads a single file to Google Drive cloud storage.
* Using a Google service account (https://developers.google.com/api-client-library/dotnet/get_started#service-accounts)
* Grants permission to this file to another Google Drive account.

## Usage

```
Jaxx.FileSync.exe ["PATH_TO_CERTFILE\key.p12"] ["YOUR_SERVICE_ACCOUNT@gserviceaccount.com"] ["DRIVEUSER@googlemail.com"] ["UPLOAD_FILE"] ["root"]
```
