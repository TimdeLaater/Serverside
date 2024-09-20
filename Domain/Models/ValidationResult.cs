using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }

        public ValidationResult()
        {
            IsValid = true; // Default to valid
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }
    }

}
