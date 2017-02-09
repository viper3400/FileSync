# FileSync

* FileSync is very basic implementation of .Net GoogleDriveAPI v3. 
* It Uploads a single file to Google Drive cloud storage, using a Google [service account](https://developers.google.com/api-client-library/dotnet/get_started#service-accounts).
* It grants permission to this file to another Google Drive account.
* It is shipped with a tiny command line application.
* For the moment it is *just* an uploader, not a file syncing tool at all.

[![Build status](https://ci.appveyor.com/api/projects/status/bn5jakeg7dkk3uhf/branch/master?svg=true)](https://ci.appveyor.com/project/viper3400/filesync/branch/master)

[Download latest release](https://github.com/viper3400/FileSync/releases/latest)

**Table Of Contents**
<!-- TOC depthFrom:2 -->

- [Prerequisites](#prerequisites)
- [Command Line Usage](#command-line-usage)
    - [Global Options and Commands](#global-options-and-commands)
        - [Example](#example)
        - [Global Options in Detail](#global-options-in-detail)
    - [Upload a File](#upload-a-file)
        - [Example](#example-1)
        - [Options for Creating a File in Detail](#options-for-creating-a-file-in-detail)
    - [Create a Folder](#create-a-folder)
        - [Example](#example-2)
        - [Options for Creating a Folder in Detail](#options-for-creating-a-folder-in-detail)
    - [List contents](#list-contents)
        - [Example](#example-3)
- [Scenario](#scenario)
- [Automation / Periodical Execution](#automation--periodical-execution)
- [Enhancements](#enhancements)
- [Build and Run from Source](#build-and-run-from-source)

<!-- /TOC -->

## Prerequisites

* The released version is tested with Windows 10.
* The released version requires .NET Version 4.6.1
* You will need to create a google service account. Read [this](https://developers.google.com/identity/protocols/OAuth2ServiceAccount#creatinganaccount).
* A service accounts is a different account, it's not your own Google account.
* Service accounts have their own 15 GB Drive free space.
* It's not visible via a web interface or somewhere else (unless you get a list via API, which is not implemented in FileSync at the moment).

## Command Line Usage

### Global Options and Commands

```
Jaxx.FileSync.exe -c ["PATH_TO_CERTFILE\key.p12"] -s ["YOUR_SERVICE_ACCOUNT@gserviceaccount.com"] [COMMANDS] [COMMANDS_OPTIONS]
```

* **Global options are mandatory!**

#### Example

```
Jaxx.FileSync.exe -c "C:\Users\mike\privatekeys\mikeskey.p12" -s "mikesservice@gserviceaccount.com" list
```

#### Global Options in Detail

* -c ["PATH_TO_CERTFILE\key.p12"] 
  * Provide the full path and filename of the private certificate p12 file you've downloaded from google api 
    console after you've created the service account.
* -s ["YOUR_SERVICE_ACCOUNT@gserviceaccount.com"] 
  * Provide the mail account you've created for the service account.
* [COMMANDS] [COMMANDS_OPTIONS]
  * commands and subcommands as *list*, *create file*, *delete object* (as follows)

### Upload a File

```
Jaxx.FileSync.exe 
      [GLOBAL_OPTIONS]
      create file
      -g ["DRIVEUSER@googlemail.com"] 
      -f ["UPLOAD_FILE"]
      -d ["root"]
```
* **All options are mandatory!**
* You have to use the -g option with your own drive space, otherwise you'd never ever have the chance to see your uploaded file again.

#### Example

```
Jaxx.FileSync.exe 
      -c "C:\Users\mike\privatekeys\mikeskey.p12"
      -s "mikesservice@gserviceaccount.com"
      create file
      -g "mikesprivategmailadress@googlemail.com" 
      -f "C:\Users\mike\Documents\list.txt"
      -d "root"
```

#### Options for Creating a File in Detail

* [GLOBAL_OPTIONS]
  * see there
* create
  * Simply calls the command to create (upload) an object.
* file
  * Tells the create command to create (upload) a file object.
* -g ["DRIVEUSER@googlemail.com"] 
  * Grant permissions to the created object to another google drive user, so that he will be able 
  to see the files in his own account's shared folder (grant it to yourself!)
* -f ["UPLOAD_FILE"]
  * Full path to the file you'd like to upload.
* -d ["root"]
  * The folder where the file should be created. The folder has to exist. 
  * Pass "root" if the file should be created in the root folder.

### Create a Folder

Starting with release 1.1.0 you can create folders in you drive storage. 

```
Jaxx.FileSync.exe 
      [GLOBAL_OPTIONS]
      create folder
      -g ["DRIVEUSER@googlemail.com"] 
      -n ["NEW_FOLDER_NAME"]
      -p ["PARENTFOLDER"]
```

* **All options are mandatory!**
* If you want to create folders in the root path, pass 'root' as parent folder.
* A parent folder has to exist if you want to create a child folder.
* Be aware that folder names need to be unique in your store, due to my hasty implementation.

#### Example

```
Jaxx.FileSync.exe 
      -c "C:\Users\mike\privatekeys\mikeskey.p12"
      -s "mikesservice@gserviceaccount.com"
      create folder
      -g "mikesprivategmailadress@googlemail.com" 
      -n "SharedDocuments"
      -p "root"
```

#### Options for Creating a Folder in Detail

* [GLOBAL_OPTIONS]
  * see there
* create
  * Simply calls the command to create an object.
* folder
  * Tells the create command to create a folder object.
* -g ["DRIVEUSER@googlemail.com"] 
  * Grant permissions to the created object to another google drive user, so that he will be able 
  to see the object in his own account's shared folder (grant it to yourself!)
* -n ["NEW_FOLDER_NAME"]
  * Name of the new folder
* -p ["PARENTFOLDER"]
  * Name of the parent folder ('root' for root folder)

### List contents

Starting with release 1.2.0 you can list the content in you drive storage. 

```
Jaxx.FileSync.exe [GLOBAL_OPTIONS] list
```

This will list all obejcts available in your store with the pattern:

```
[ObjectId] [mimeType] -> filename.ext, lastmodified
```

#### Example

```
Jaxx.FileSync.exe -c "C:\Users\mike\privatekeys\mikeskey.p12" -s "mikesservice@gserviceaccount.com" list
```

## Scenario
Mike runs a little MySQL database on a Windows OS. Each night it creates a dump of the database to backup data.
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

He created a little batch file which first creates the backup of the MySQL database and uploads the backuped
file in a next script. The batch file is trigger via build in [Windows Task Scheduler](https://technet.microsoft.com/en-us/library/cc721931(v=ws.11).aspx).

After a while his store grows and grows as more and more backups are uploaded. He decides, that he only wants to keep the backups from the last 30 days. But wait!
What if the backups won't run for some days and Mike didn't notice this for any reason? So he decided, that he always want to keep the last 10 files.

## Automation / Periodical Execution

As Mike in our scenario: Use the [Windows Task Scheduler](https://technet.microsoft.com/en-us/library/cc721931(v=ws.11).aspx) to automate FileSync.

## Enhancements

I plan to implement a "houskeeping" feature in a next step. 
This should add a deletion feature which "cleans" files having reached a certain age. 

This is all I need for the moment. Check the issues page on GitHub for further "nice to haves", feel free to create issues or to contribute.

## Build and Run from Source

* Microsoft Visual Studio 2015 Community Edition
* .NET Core

To build & run, run _dotnet restore_ and _dotnet run_.

