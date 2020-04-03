using System.Text;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;

namespace PQCreator
{
    static class utility
    {
        private static StreamWriter SW;
        private static string StartZipPath;

        public static bool UnzipFile(string nomeFileZip, string BaseDir)
        {
            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(nomeFileZip)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {

                        string directoryName = Path.GetDirectoryName(Path.Combine(BaseDir, theEntry.Name));
                        string fileName = Path.GetFileName(theEntry.Name);

                        // create directory
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(Path.Combine(BaseDir, theEntry.Name)))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static void writelog(string s)
        {
            if (SW == null ) SW = new StreamWriter("log.txt");
            SW.WriteLine(DateTime.Now + " " + s + Environment.NewLine);
            SW.Flush();

        }

        /// <summary>
        /// Creates a zip file
        /// </summary>
        /// <param name="zipFileStoragePath">where to store the zip file</param>
        /// <param name="zipFileName">the zip file filename</param>
        /// <param name="fileToZip">the file to zip</param>
        /// <returns>indicates whether the file was created successfully</returns>
        public static bool CreateZipFile(string zipFileStoragePath
            , string zipFileName
            , FileInfo fileToZip)
        {
            return CreateZipFile(zipFileStoragePath
                                , zipFileName
                                , (FileSystemInfo)fileToZip);
        }

        /// <summary>
        /// Creates a zip file
        /// </summary>
        /// <param name="zipFileStoragePath">where to store the zip file</param>
        /// <param name="zipFileName">the zip file filename</param>
        /// <param name="directoryToZip">the directory to zip</param>
        /// <returns>indicates whether the file was created successfully</returns>
        public static bool CreateZipFile(string zipFileStoragePath
            , string zipFileName
            , DirectoryInfo directoryToZip)
        {
            StartZipPath=directoryToZip.FullName;
            return CreateZipFile(zipFileStoragePath
                                , zipFileName
                                , (FileSystemInfo)directoryToZip);
        }

        /// <summary>
        /// Creates a zip file
        /// </summary>
        /// <param name="zipFileStoragePath">where to store the zip file</param>
        /// <param name="zipFileName">the zip file filename</param>
        /// <param name="fileSystemInfoToZip">the directory/file to zip</param>
        /// <returns>indicates whether the file was created successfully</returns>
        public static bool CreateZipFile(string zipFileStoragePath
            , string zipFileName
            , FileSystemInfo fileSystemInfoToZip)
        {
            return CreateZipFile(zipFileStoragePath
                                , zipFileName
                                , new FileSystemInfo[] 
                            { 
                                fileSystemInfoToZip 
                            });
        }

        /// <summary>
        /// A function that creates a zip file
        /// </summary>
        /// <param name="zipFileStoragePath">location where the file should be created</param>
        /// <param name="zipFileName">the filename of the zip file</param>
        /// <param name="fileSystemInfosToZip">an array of filesysteminfos that needs to be added to the file</param>
        /// <returns>a bool value that indicates whether the file was created</returns>
        public static bool CreateZipFile(string zipFileStoragePath
            , string zipFileName
            , FileSystemInfo[] fileSystemInfosToZip)
        {
            // a bool variable that says whether or not the file was created
            bool isCreated = false;

            try
            {
                //create our zip file
                ZipFile z = ZipFile.Create(Path.Combine(zipFileStoragePath,zipFileName));
                //initialize the file so that it can accept updates
                z.BeginUpdate();
                //get all the files and directory to zip
                GetFilesToZip(fileSystemInfosToZip, z);
                //commit the update once we are done
                z.CommitUpdate();
                //close the file
                z.Close();
                //success!
                isCreated = true;
            }
            catch (Exception ex)
            {
                //failed
                isCreated = false;
                //lets throw our error
                throw ex;
            }

            //return the creation status
            return isCreated;
        }

        /// <summary>
        /// Iterate thru all the filesysteminfo objects and add it to our zip file
        /// </summary>
        /// <param name="fileSystemInfosToZip">a collection of files/directores</param>
        /// <param name="z">our existing ZipFile object</param>
        private static void GetFilesToZip(FileSystemInfo[] fileSystemInfosToZip, ZipFile z)
        {
            //check whether the objects are null
            if (fileSystemInfosToZip != null && z != null)
            {
                //iterate thru all the filesystem info objects


                //prima i file poi le cartelle (per EPUB)
                Array.Sort<FileSystemInfo>(fileSystemInfosToZip, new Comparison<FileSystemInfo>(compareFiles));

                foreach (FileSystemInfo fi in fileSystemInfosToZip)
                {
                    //check if it is a directory
                    if (fi is DirectoryInfo)
                    {
                        DirectoryInfo di = (DirectoryInfo)fi;
                        //add the directory

                        //z.AddDirectory(di.FullName);
                        if (fi.FullName.Length>StartZipPath.Length)
                            z.AddDirectory(fi.FullName.Substring(StartZipPath.Length + 1));

                        //drill thru the directory to get all
                        //the files and folders inside it.
                        GetFilesToZip(di.GetFileSystemInfos(), z);
                    }
                    else
                    {
                        //add it
                        z.Add(fi.FullName,fi.FullName.Substring(StartZipPath.Length + 1));
                    }
                }
            }
        }

        private static int compareFiles(FileSystemInfo x, FileSystemInfo y)
        {
            if (x is FileInfo) return 1;
            else return 0;
        }

        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
