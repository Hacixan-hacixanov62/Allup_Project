using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Core.Attributes
{
    public class MaxSizeAttribute:ValidationAttribute
    {
        private int _size;
        public MaxSizeAttribute(int size)
        {
            _size = size;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<IFormFile> files = new List<IFormFile>();
            if (value is List<IFormFile> fileList) files = fileList;
            if (value is IFormFile file) files.Add(file);
            foreach (var item in files)
            {
                if (item.Length > _size)
                {
                    string gmessage = "file size must be less than 2mb";
                    return new ValidationResult(gmessage);
                }
            }


            return ValidationResult.Success;
        }
    }
}
