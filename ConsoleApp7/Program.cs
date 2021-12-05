using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp7
{
    class Program
    {
        private static IEnumerable<string> Traverse(string rootDirectory)
        {
            
            IEnumerable<string> files = Enumerable.Empty<string>();
            IEnumerable<string> directories = Enumerable.Empty<string>();
            try
            {
                // The test for UnauthorizedAccessException.
                var permission = new FileIOPermission(FileIOPermissionAccess.PathDiscovery, rootDirectory);
                permission.Demand();

                files = Directory.GetFiles(rootDirectory);
                directories = Directory.GetDirectories(rootDirectory);
            }
            catch
            {
                // Ignore folder (access denied).
                rootDirectory = null;
              
            }

            if (rootDirectory != null)
                yield return rootDirectory;

            foreach (var file in files)
            {
                yield return file;
            }

            // Recursive call for SelectMany.
            var subdirectoryItems = directories.SelectMany(Traverse);
            foreach (var result in subdirectoryItems)
            {
                yield return result;
            }
           
        }

        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                Console.WriteLine("Scanning start. Please wait...");

                //     Сканирование файлов ПК
                var paths = Traverse(@"c:\");
                string[] stringPath = paths.ToArray();

                List<string> searchedFile = new List<string>();

                //     Поиск нужных файлов
                Console.WriteLine("\nSearching for your file...");
                for (int i = 0; i < stringPath.Length; i++)
                {
                    if (stringPath[i].Contains(args[0]))
                        searchedFile.Add(stringPath[i]);
                }
                Console.WriteLine("\nFiles found:");
                searchedFile.ForEach(Console.WriteLine);

                //     Создание маассива новых наименований
                string[] newFileName = new string[searchedFile.Count];
                for (int i = 0; i < newFileName.Length; i++)
                {
                    newFileName[i] = string.Empty;
                }

                //   Создание нового пути для файла
                for (int j = 0; j < searchedFile.Count; j++)
                {
                    for (int i = 0; i < searchedFile[j].Split('\\').Length - 1; i++)
                    {
                        newFileName[j] += searchedFile[j].Split('\\')[i] + '\\';
                    }
                }

                //   Создание нового имени для файла
                for (int i = 0; i < newFileName.Length; i++)
                {
                    newFileName[i] += args[1];
                }

                //   Переименование файлов
                for (int i = 0; i < newFileName.Length; i++)
                    File.Move(searchedFile[i], newFileName[i]);

                Console.WriteLine("\nFiles Renamed!");
            }
        }
       
    }
}
