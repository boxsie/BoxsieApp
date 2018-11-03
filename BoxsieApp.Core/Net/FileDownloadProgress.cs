using System;

namespace BoxsieApp.Core.Net
{
    public class FileDownloadProgress
    {
        public long TotalBytes { get; set; }
        public long CurrentBytes { get; set; }
        public double TotalSeconds { get; set; }
         
        public double Percent => ((double)CurrentBytes / TotalBytes) * 100;
        public TimeSpan RemainingTime => TimeSpan.FromSeconds(((TotalSeconds / Percent) * 100) - TotalSeconds);
        public double MegabytesPerSecond => (CurrentBytes / TotalSeconds) / (int)1E+6;
    }
}