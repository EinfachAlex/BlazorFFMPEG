using EinfachAlex.Utils.HashGenerator;
using EinfachAlex.Utils.Logging;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class AutoEncodeFolder
    {
        public static AutoEncodeFolder constructNew(databaseContext databaseContext, string encoder, string inputPath, string outputPath, string qualityMethod, long qualityValue, bool commit)
        {
            Hash id = generateId(inputPath, outputPath);

            AutoEncodeFolder proxyObject = new AutoEncodeFolder()
            {
                Inputpath = inputPath,
                Outputpath = outputPath
            };

            var proxy = databaseContext.CreateProxy<AutoEncodeFolder>();
            databaseContext.Entry(proxy).CurrentValues.SetValues(proxyObject);

            databaseContext.Add(proxy);

            if (commit) databaseContext.SaveChanges();

            return proxy;
        }

        private static Hash generateId(string inputPath, string outputPath)
        {
            string key = $"{inputPath}{outputPath}";

            Hash id = HashGenerator.generateSHA256(key);
            LoggerCommonMessages.logGeneratedId(id);

            return id;
        }
    }
}