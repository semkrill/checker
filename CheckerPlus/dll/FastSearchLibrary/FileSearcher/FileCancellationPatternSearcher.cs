using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FastSearchLibrary
{
    internal class FileCancellationPatternSearcher : FileCancellationSearcherBase
    {

        private string pattern;

        public FileCancellationPatternSearcher(string folder, string pattern, CancellationToken token, ExecuteHandlers handlerOption, bool suppressOperationCanceledException)
            : base(folder, token, handlerOption, suppressOperationCanceledException)
        {
            this.pattern = pattern;
        }


        protected override void GetFiles(string folder)
        {
            token.ThrowIfCancellationRequested();

            DirectoryInfo dirInfo = null;
            DirectoryInfo[] directories = null;

            try
            {
                dirInfo = new DirectoryInfo(folder);
                directories = dirInfo.GetDirectories();

                if (directories.Length == 0)
                {
                    var resFiles = dirInfo.GetFiles(pattern);
                    if (resFiles.Length > 0)
                        OnFilesFound(resFiles.ToList());

                    return;
                }
            }
            catch (UnauthorizedAccessException )
            {
                return;
            }
            catch (PathTooLongException )
            {
                return;
            }
            catch (DirectoryNotFoundException )
            {
                return;
            }

            foreach (var d in directories)
            {
                token.ThrowIfCancellationRequested();

                GetFiles(d.FullName);
            }

            token.ThrowIfCancellationRequested();

            try
            {
                var resFiles = dirInfo.GetFiles(pattern);
                if (resFiles.Length > 0)
                    OnFilesFound(resFiles.ToList());
            }
            catch (UnauthorizedAccessException )
            {
            }
            catch (PathTooLongException )
            {
            }
            catch (DirectoryNotFoundException )
            {
            }
        }



        protected override List<DirectoryInfo> GetStartDirectories(string folder)
        {
            token.ThrowIfCancellationRequested();

            DirectoryInfo dirInfo = null;
            DirectoryInfo[] directories = null;
            try
            {
                dirInfo = new DirectoryInfo(folder);
                directories = dirInfo.GetDirectories();

                var resFiles = dirInfo.GetFiles(pattern);
                if (resFiles.Length > 0)
                    OnFilesFound(resFiles.ToList());

                if (directories.Length > 1)
                    return new List<DirectoryInfo>(directories);

                if (directories.Length == 0)
                    return new List<DirectoryInfo>();
            }
            catch (UnauthorizedAccessException )
            {
                return new List<DirectoryInfo>();
            }
            catch (PathTooLongException )
            {
                return new List<DirectoryInfo>();
            }
            catch (DirectoryNotFoundException )
            {
                return new List<DirectoryInfo>();
            }

            return GetStartDirectories(directories[0].FullName);
        }

    }
}
