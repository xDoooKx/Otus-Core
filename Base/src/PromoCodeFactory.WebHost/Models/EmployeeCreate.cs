using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using PromoCodeFactory.Core.Abstractions.Tools;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeCreate: IClassValidator
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool Validate()
        {
            if (String.IsNullOrWhiteSpace(FirstName)) return false;
            if (String.IsNullOrWhiteSpace(LastName)) return false;
            if (String.IsNullOrWhiteSpace(Email)) return false;

            return true;
        }
    }
}