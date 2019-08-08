using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading.Tasks;
using WebUploadServer.Models;
using AI3.Server.Common.Entity;
using Microsoft.Extensions.Options;
using WebUploadServer.BLL;
using WebUploadServer.BLL.Common;

namespace WebUploadServer.Bll
{
    public class UploadBll
    {
        private readonly Custom _customSettings;

        public UploadBll(IOptions<Custom> customSettings)
        {
            _customSettings = customSettings.Value;
        }

        public async Task<UploadTask> MultipartUpload(UploadTask dto)
        {
            try
            {
                if (dto.Status == "init")
                {
                    dto.FilePath = dto.CreateRelativePath();
                    var pathes = Path.GetDirectoryName(dto.FilePath).Split(Path.DirectorySeparatorChar);
                    string tempRelativePath = _customSettings.FilePath;
                    foreach (string path in pathes)
                    {
                        tempRelativePath = Path.Combine(tempRelativePath, path);
                        string direSubPath = tempRelativePath;
                        if (!Directory.Exists(direSubPath))
                        {
                            Directory.CreateDirectory(direSubPath);
                            ChmodAccess(direSubPath);
                        }
                    }
                    dto.FilePath = Path.Combine(_customSettings.FilePath, dto.FilePath);
                    dto.Status = "uploading";
                }
                if (dto.Status == "uploading")
                {
                    await Upload(dto);
                }
                if (dto.ChunkIndex == dto.ChunkTotal - 1)
                {
                    await CompleteMultipartUpload(dto.FilePath);
                    Import(dto);
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
            var absolutePath = multipartModel.FilePath;
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
                throw new Exception($"上传失败文件{multipartModel.FileName}到{multipartModel.FilePath}失败", ex);
            }
        }

        public async Task CompleteMultipartUpload(string absolutePath)
        {
            ChmodAccess(absolutePath);
            await Task.CompletedTask;
        }

        public void Import(UploadTask task)
        {
            var entityType = EntityTypeHelper.GetEntityTypeByCode(task.EntityType);
            var ext = Path.GetExtension(task.FilePath).Replace(".", "").ToUpper();
            var request = new TemporaryEntity
            {
                TypeID = task.EntityType,
                ContentID = task.FileId,
                EntityData = new EntityDataType
                {
                    Items = new List<ItemType>
                    {
                        new ItemType
                        {
                            Code = "name",
                            Value = Path.GetFileNameWithoutExtension(task.FilePath)
                        },
                        new ItemType
                        {
                            Code = "contentid",
                            Value = task.FileId
                        },
                        new ItemType
                        {
                            Code = "UMID",
                            Value = ""
                        },
                        new ItemType
                        {
                            Code = "source",
                            Value = "Mediaview"
                        },
                        new ItemType
                        {
                            Code = "entityType",
                            Value = task.EntityType
                        },
                        new ItemType
                        {
                            Code = "version",
                            Value = ""
                        },
                        new ItemType
                        {
                            Code = "iconIndex",
                            Value = "0"
                        },
                        new ItemType
                        {
                            Code = "privilegeTemplateCode",
                            Value = "Public"
                        },
                        new ItemType
                        {
                            Code = "CreatorCode",
                            Value = "admin"
                        },
                        new ItemType
                        {
                            Code = "programForm",
                            Value = entityType.ProgramForm
                        },
                        new ItemType
                        {
                            Code = "entityTypeId",
                            Value = entityType.TypeId.ToString()
                        }
                    }
                },
                FileData = new FileDataType
                {
                    Files = new List<FileInfoType>
                    {
                        new FileInfoType
                        {
                            GUID = task.FileId,
                            Name = task.FileName,
                            FileType = "FILETYPE_"+ext,
                            InstanceList = new InstanceList
                            {
                                Instances = new List<FilePathType>
                                {
                                    new FilePathType
                                    {
                                        MAMDevice = "HiResCache",
                                        StorageType = "diskfile",
                                        Location = new LocationType
                                        {
                                            FullPath = task.FilePath
                                        }
                                    }
                                }
                            },
                            QualityType = "0",
                            Length = task.Size,
                            StateInfo = "0",
                        }
                    }
                }
            };
            var reqXml = XmlHelper.XmlSerialize(request);
            var res = new WebService(_customSettings.WebServiceAdress).Import(reqXml);
        }

        private void ChmodAccess(string path)
        {
            FileIOPermission fileIoPermission = new FileIOPermission(PermissionState.None);
            fileIoPermission.AddPathList(FileIOPermissionAccess.AllAccess, path);
            fileIoPermission.Demand();
        }


    }
}