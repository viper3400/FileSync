using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jaxx.FileSync.GoogleDrive;
using Jaxx.FileSync.Shared.Interfaces;


namespace Jaxx.FileSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var certFile = args[0];
                var serviceAccountMail = args[1];
                var userName = args[2];
                var uploadFile = args[3];
                var uploadFileFolder = args[4];

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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.WriteLine("Press a key to continue ...");
            //Console.ReadLine();
        }
    }
}
