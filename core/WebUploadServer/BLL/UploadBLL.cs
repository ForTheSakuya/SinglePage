using System;
using System.IO;
using System.Security.Permissions;
using System.Threading.Tasks;
using WebUploadServer.Models;

namespace WebUploadServer.Bll
{
    public class UploadBll
    {
        // 统一分片大小
        // private const int chunkSize = 20 * 1024 * 1024;
        private string filePath = "E:\\ext_file_root\\text";

        public async Task<UploadTask> MultipartUpload(UploadTask dto)
        {
            try
            {
                if (dto.Status == "init")
                {
                    dto.FilePath = filePath;
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                        ChmodAccess(filePath);
                    }
                    dto.Status = "uploading";
                }
                if (dto.Status == "uploading")
                {
                    await Upload(dto);
                }
                if (dto.ChunkIndex == dto.ChunkTotal - 1)
                {
                    await CompleteMultipartUpload(dto.FileName);
                    dto.Status = "done";
                }
            }
            catch (Exception)
            {
                dto.Status = "error";
            }
            return dto;
        }

        public async Task Upload(UploadTask multipartModel)
        {
            var absolutePath = Path.Combine(filePath, multipartModel.FileName);
            string directoryName = Path.GetDirectoryName(absolutePath);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            Stream stream = multipartModel.FileData.OpenReadStream();

            try
            {
                using (var writeStream = File.Open(absolutePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    writeStream.Position = multipartModel.ChunkIndex * multipartModel.ChunkSize;
                    await stream.CopyToAsync(writeStream, 64 * 1024);
                    await writeStream.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"��Ƭ�ϴ��ļ�{multipartModel.FileName}��{multipartModel.FilePath}ʧ��", ex);
            }
        }

        public async Task CompleteMultipartUpload(string fileName)
        {
            ChmodAccess(Path.Combine(filePath, fileName));
            await Task.CompletedTask;
        }

        private void ChmodAccess(string path)
        {
            FileIOPermission fileIoPermission = new FileIOPermission(PermissionState.None);
            fileIoPermission.AddPathList(FileIOPermissionAccess.AllAccess, path);
            fileIoPermission.Demand();
        }

    }
}