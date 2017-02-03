# FileSync

* FileSync is very basic implementation of .Net GoogleDriveAPI v3. 
* It Uploads a single file to Google Drive cloud storage, using a Google [service account](https://developers.google.com/api-client-library/dotnet/get_started#service-accounts).
* It grants permission to this file to another Google Drive account.
* Is shipped with a tiny command line application

## Command Line Usage

```
Jaxx.FileSync.exe 
      -c ["PATH_TO_CERTFILE\key.p12"] 
      -s ["YOUR_SERVICE_ACCOUNT@gserviceaccount.com"] 
      create file
      -g ["DRIVEUSER@googlemail.com"] 
      -f ["UPLOAD_FILE"]
      -d ["root"]
```

* **All options are mandatory!**
* You will need to create a google service account. Read [this](https://developers.google.com/identity/protocols/OAuth2ServiceAccount#creatinganaccount).
* Service accounts have their own 15 GB Drive free space.
* It's not visible via a web interface or somewhere else (unless you get a list via API, which is not implemented).
* **You have to use the -g option with your own drive space, otherwise you'd never ever have the chance to see your uploaded file again.**

### Options

* -c ["PATH_TO_CERTFILE\key.p12"] 
  * Provide the full path and filename of the private certificate p12 file you've downloaded from google api 
    console after you've created the service account.
* -s ["YOUR_SERVICE_ACCOUNT@gserviceaccount.com"] 
  * Provide the mail account you've created for the service account.
* create
  * Simply calls the command to create (upload) an object.
* file
  * Tells the create command to create (upload) a file object.
* -g ["DRIVEUSER@googlemail.com"] 
  * Grant permissions to the created object to another google drive user, so that he will be able 
  to see the files in his own account's shared folder (grant it to yourself!)
* -d ["root"]
  * The folder where the file should be created. The folder has to exist. 
  * Pass "root" if the file should be created in the root folder.
  * This is the only supported folder for now ...

## Scenario
Mike runs a little MySql database on a Windows OS. Each night it creates a dump of the database to backup data.
The drawback: Until now this backups were stored on the local hard drive. What happens if this disk crashes?

After throwing away other ideas Mike decided to upload an extra backup to Google Drive.
But he didn't want to install Google Drive app on his server for reasons:

* Neither, he wants to log in with his own google account 
* Nor, he wants to create a google account for the server user
* Nor, it would be guaranteed that the user is logged into the system all the time at all

He decided to create a [google service account](https://developers.google.com/api-client-library/dotnet/get_started#service-accounts),
authenticated by an email account and a private certification file (key.p12). 
To access his uloaded backups he grants his own google account the necessary permissions, so it will be 
visible in his own cloud drive in "shared files".

## Enhancments

I plan to implement a "houskeeping" feature in a next step. 
This should add a deletion feature which "cleans" files having reached a certain age. 
Also I will implement a create folder function. 

This is all I need for the moment. Check the issues page on GitHub for further "nice to haves", feel free to create issues or to contribute.
