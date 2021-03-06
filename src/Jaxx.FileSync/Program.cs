﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jaxx.FileSync.GoogleDrive;
using Jaxx.FileSync.Shared.Interfaces;
using Microsoft.Extensions.CommandLineUtils;

namespace Jaxx.FileSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // prepare the di container
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<GoogleDriveModule>();
            var iocContainer = containerBuilder.Build();

            // prepare command line application
            CommandLineApplication app =
                new CommandLineApplication(throwOnUnexpectedArg: false);

            var certFile = app.Option("-c | --certFile", "Path to key.p12 certificate (mandatory).", CommandOptionType.SingleValue);
            var serviceAccountMail = app.Option("-s | --serviceAccount", "Google service account mail (mandatory).", CommandOptionType.SingleValue);

            ConfigureCommandLineApp(iocContainer, app, certFile, serviceAccountMail);

            app.OnExecute(() =>
            {
                if (!certFile.HasValue() && !serviceAccountMail.HasValue())
                {
                    Console.WriteLine($"A mandatory value was not set.");
                    app.ShowHelp();
                    return 1;
                }

                return 0;
            });
            app.HelpOption("-? | -h | --help");
            try
            {
                var result = app.Execute(args);
            }
            catch (CommandParsingException e)
            {
                Console.WriteLine(e.Message);
            }


            //Console.WriteLine("Press a key to continue ...");
            //Console.ReadLine();
        }

        private static void ConfigureCommandLineApp(IContainer iocContainer, CommandLineApplication app, CommandOption certFile, CommandOption serviceAccountMail)
        {
            ConfigureCommandLineCreateCommand(iocContainer, app, certFile, serviceAccountMail);
            ConfigureCommandLineDeleteCommand(iocContainer, app, certFile, serviceAccountMail);
            ConfigureCommandLineListCommand(iocContainer, app, certFile, serviceAccountMail);
        }

        private static void ConfigureCommandLineListCommand(IContainer iocContainer, CommandLineApplication app, CommandOption certFile, CommandOption serviceAccountMail)
        {
            var list = app.Command("list", config =>
            {
                config.OnExecute(() =>
                {
                    ReadFiles(iocContainer, certFile.Value(), serviceAccountMail.Value());
                    //config.ShowHelp(); //show help
                    return 0;
                });
                config.HelpOption("-? | -h | --help"); //show help on --help

            });
        }


        private static void ConfigureCommandLineDeleteCommand(IContainer iocContainer, CommandLineApplication app, CommandOption certFile, CommandOption serviceAccountMail)
        {
            var delete = app.Command("delete", config =>
            {
                config.OnExecute(() =>
                {
                    config.ShowHelp(); //show help
                    return 1; //return error since we didn't do anything
                });
                config.HelpOption("-? | -h | --help"); //show help on --help
            });

            delete.Command("agedfiles", config =>
            {
                config.Description = "Delete files wich haven't changed since a certain time span.";
                config.HelpOption("-? | -h | --help");
                var fileAge = config.Option("-a | --AgeInDays", "The time span in days (mandatory).", CommandOptionType.SingleValue);
                var folder = config.Option("-f | --folder", "The folder in which the files should be deleted (mandatory).", CommandOptionType.SingleValue);
                var filesToKeepAtLeast = config.Option("-k | --keepFiles", "The number of files you want to keep at least (optional)", CommandOptionType.SingleValue);
                var preview = config.Option("-p | --preview", "Don't delete anything, just preview what would happen (optional).", CommandOptionType.NoValue);

                config.OnExecute(() =>
                {
                    if (certFile.HasValue() && serviceAccountMail.HasValue() && fileAge.HasValue() && folder.HasValue())
                    {
                        int keepFiles = 0;
                        int.TryParse(filesToKeepAtLeast.Value(), out keepFiles);
                        DeleteAgedFiles(iocContainer, certFile.Value(), serviceAccountMail.Value(), fileAge.Value(), folder.Value(), keepFiles, preview.HasValue());
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"A mandatory value was not set.");
                        config.ShowHelp();
                        return 1;
                    }
                });
            });

            delete.Command("object", config =>
            {
                config.Description = "Delete the object with the given id.";
                config.HelpOption("-? | -h | --help");
                var objectId = config.Option("-i | --id", "The id of the object which should be deleted", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (certFile.HasValue() && serviceAccountMail.HasValue() && objectId.HasValue())
                    {
                        DeleteObject(iocContainer, certFile.Value(), serviceAccountMail.Value(), objectId.Value());
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"A mandatory value was not set.");
                        config.ShowHelp();
                        return 1;
                    }
                }
                );
            });
        }



        private static void ConfigureCommandLineCreateCommand(IContainer iocContainer, CommandLineApplication app, CommandOption certFile, CommandOption serviceAccountMail)
        {
            var create = app.Command("create", config =>
            {
                config.OnExecute(() =>
                {
                    config.ShowHelp(); //show help
                    return 1; //return error since we didn't do anything
                });
                config.HelpOption("-? | -h | --help"); //show help on --help
            });

            create.Command("file", config =>
            {
                config.Description = "Create a remote file (upload).";
                config.HelpOption("-? | -h | --help");
                var permissionList = config.Option("-g | --grantPermission", "Grant permisssion to the created object (mandatory).", CommandOptionType.SingleValue);
                var uploadFile = config.Option("-f | --file", "Path to upload file (mandatory).", CommandOptionType.SingleValue);
                var uploadFileFolder = config.Option("-d | --uploadDir", "Name of the remote dir (mandatory).", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (certFile.HasValue() && serviceAccountMail.HasValue() && permissionList.HasValue() && uploadFile.HasValue() && uploadFileFolder.HasValue())
                    {
                        CreateFile(iocContainer, certFile.Value(), serviceAccountMail.Value(), permissionList.Value(), uploadFile.Value(), uploadFileFolder.Value());
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"A mandatory value was not set.");
                        config.ShowHelp();
                        return 1;
                    }

                });
            });

            create.Command("folder", config =>
            {
                config.Description = "Create a remote folder (directory).";
                config.HelpOption("-? | -h | --help");
                var permissionList = config.Option("-g | --grantPermission", "Grant permisssion to the created object (mandatory).", CommandOptionType.SingleValue);
                var newFolderName = config.Option("-n | --name", "Name of the new folder (mandatory).", CommandOptionType.SingleValue);
                var parentFolderName = config.Option("-p | --parentFolder", "Name of the parent folder ('root' for root folder) (mandatory).", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (certFile.HasValue() && serviceAccountMail.HasValue() && permissionList.HasValue() && newFolderName.HasValue() && parentFolderName.HasValue())
                    {
                        CreateFolder(iocContainer, certFile.Value(), serviceAccountMail.Value(), permissionList.Value(), newFolderName.Value(), parentFolderName.Value());
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine($"A mandatory value was not set.");
                        config.ShowHelp();
                        return 1;
                    }

                });
            });
        }

        private static void CreateFile(IContainer iocContainer, string certFile, string serviceAccountMail, string userName, string uploadFile, string uploadFileFolder)
        {
            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var userlist = new List<string> { userName };
                var accountProvider = scope.Resolve<IGoogleAccountProvider>(
                    new NamedParameter("certFile", certFile),
                    new NamedParameter("serviceAccountEmail", serviceAccountMail));

                var uploader = scope.Resolve<IUploader>(
                    new TypedParameter(typeof(IEnumerable<string>), userlist),
                    new TypedParameter(typeof(IGoogleAccountProvider), accountProvider));

                uploader.UploadFile(uploadFile, uploadFileFolder);
            }
        }

        private static void CreateFolder(IContainer iocContainer, string certFile, string serviceAccountMail, string userName, string folderName, string parentFolderName)
        {
            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var userlist = new List<string> { userName };
                var accountProvider = scope.Resolve<IGoogleAccountProvider>(
                    new NamedParameter("certFile", certFile),
                    new NamedParameter("serviceAccountEmail", serviceAccountMail));

                var folderController = scope.Resolve<IFolderController>(
                    new TypedParameter(typeof(IEnumerable<string>), userlist),
                    new TypedParameter(typeof(IGoogleAccountProvider), accountProvider));

                folderController.CreateFolder(folderName, parentFolderName);
            }
        }

        private static void DeleteAgedFiles(IContainer iocContainer, string certFile, string serviceAccountMail, string fileAge, string folderName, int filesToKeepAtLeast, bool preview)
        {
            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var accountProvider = scope.Resolve<IGoogleAccountProvider>(
                    new NamedParameter("certFile", certFile),
                    new NamedParameter("serviceAccountEmail", serviceAccountMail));

                var deleter = scope.Resolve<IDeleteController>(
                    new TypedParameter(typeof(IGoogleAccountProvider), accountProvider));

                int age;
                int.TryParse(fileAge, out age);

                deleter.DeleteAgedFiles(age, folderName, filesToKeepAtLeast, preview);
            }
        }

        private static void ReadFiles(IContainer iocContainer, string certFile, string serviceAccountMail)
        {

            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var accountProvider = scope.Resolve<IGoogleAccountProvider>(
                    new NamedParameter("certFile", certFile),
                    new NamedParameter("serviceAccountEmail", serviceAccountMail));

                var reader = scope.Resolve<IReadController>(
                     new TypedParameter(typeof(IGoogleAccountProvider), accountProvider));

                var list = reader.List();
                foreach (var file in list)
                {
                    Console.WriteLine(file);
                }

            }
            Console.WriteLine();
            Console.WriteLine("==================================");
            Console.WriteLine("Press a key to exit ... ");
            Console.ReadKey();
        }

        private static void DeleteObject(IContainer iocContainer, string certFile, string serviceAccountMail, string objectId)
        {
            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var accountProvider = scope.Resolve<IGoogleAccountProvider>(
                    new NamedParameter("certFile", certFile),
                    new NamedParameter("serviceAccountEmail", serviceAccountMail));

                var deleter = scope.Resolve<IDeleteController>(
                   new TypedParameter(typeof(IGoogleAccountProvider), accountProvider));

                deleter.DeleteObject(objectId);

            }
        }
    }
}
