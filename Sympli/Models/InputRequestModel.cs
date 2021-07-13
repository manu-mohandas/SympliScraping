using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sympli.Models
{
    public class InputRequestModel
    {
        public string SearchQuery { get; set; } 
        public string UrlText { get; set; }
    }

    public class InputRequestModelValidator : AbstractValidator<InputRequestModel>
    {
        public InputRequestModelValidator()
        {
            RuleFor(x => x.SearchQuery).NotNull();
            RuleFor(x => x.UrlText).NotNull();
        }
    }
}
