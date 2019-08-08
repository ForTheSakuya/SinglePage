using System.Collections.Generic;
using WebUploadServer.Models;

namespace WebUploadServer.BLL
{
    public static class EntityTypeHelper
    {
        public static Dictionary<string, EntityType> EntityTypes = new Dictionary<string, EntityType>
        {
            ["Clip"] = new EntityType
            {
                Code = "Clip",
                ProgramForm = "素材",
                TypeId = 2,
            },
            ["Picture"] = new EntityType
            {
                Code = "Picture",
                ProgramForm = "图片",
                TypeId = 3,
            },
            ["Document"] = new EntityType
            {
                Code = "Document",
                ProgramForm = "文档",
                TypeId = 4,
            },
            ["Other"] = new EntityType
            {
                Code = "Other",
                ProgramForm = "其他类型",
                TypeId = 7,
            },
            ["Audio"] = new EntityType
            {
                Code = "Audio",
                ProgramForm = "音频",
                TypeId = 8,
            },
        };

        public static EntityType GetEntityTypeByCode(string type)
        {
            if (EntityTypes.ContainsKey(type))
                return EntityTypes[type];
            return EntityTypes["Other"];
        }
    }
}
