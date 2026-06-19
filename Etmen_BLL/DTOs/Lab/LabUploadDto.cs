using Microsoft.AspNetCore.Http; // مهم جداً عشان الـ IFormFile يشتغل
using System;

namespace Etmen_BLL.DTOs.Lab
{
    public class LabUploadDto
    {
        public string TestName { get; set; } = string.Empty;
        public DateTime TestDate { get; set; }

        
        public IFormFile? LabFile { get; set; }

        public string? FilePath { get; set; }
        public bool UseOcr { get; set; } = true;
    }
}