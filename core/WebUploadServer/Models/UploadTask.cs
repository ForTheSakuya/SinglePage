using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WebUploadServer.Models
{
    public class UploadTask
    {
        /// <summary>
        /// init/uploading/done/error
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 文件Id
        /// </summary>
        public string FileId { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public IFormFile FileData { get; set; }

        /// <summary>
        /// 当前传输第几片
        /// </summary>
        public long ChunkIndex { get; set; }
        public long ChunkTotal { get; set; }

        public long ChunkSize { get; set; }
        /// <summary>
        /// MD5
        /// </summary>
        public string Md5 { get; set; }
        public string EntityType { get; set; }
        public long Size { get; set; }


        public string CreateRelativePath()
        {
            var now = DateTime.Now;
            return Path.Combine(EntityType,
                now.ToString("yyyyMMdd"),
                FileId,
                FileName);
        }
    }
}