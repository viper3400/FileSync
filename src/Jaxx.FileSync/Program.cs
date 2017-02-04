using System;
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
            CommandLineApplication app =
                new CommandLineApplication(throwOnUnexpectedArg: false);

            var certFile = app.Option("-c | --certFile", "Path to key.p12 certificate (mandatory).", CommandOptionType.SingleValue);
            var serviceAccountMail = app.Option("-s | --serviceAccount", "Google service account mail (mandatory).", CommandOptionType.SingleValue);

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
                        CreateFile(certFile.Value(), serviceAccountMail.Value(), permissionList.Value(), uploadFile.Value(), uploadFileFolder.Value());
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

        private static void CreateFile(string certFile, string serviceAccountMail, string userName, string uploadFile, string uploadFileFolder)
        {

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<GoogleDriveUploadModule>();
            var iocContainer = containerBuilder.Build();

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
    }
}
