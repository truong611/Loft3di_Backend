﻿namespace TN.TNM.DataAccess.Messages.Results.File
{
    public class DownloadFileResult : BaseResult
    {
        public byte[] FileAsBase64 { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
    }
}
