using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ValidationResult
    {
        public List<string> Errors { get; private set; }

        public bool IsValid => !Errors.Any();

        public ValidationResult()
        {
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }

}
