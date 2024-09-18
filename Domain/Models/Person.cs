﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }
        public ICollection<BoardGameNight> Participations { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public List<DietaryPreference> DietaryPreferences { get; set; } = new List<DietaryPreference> { DietaryPreference.NoPreference };



    }


    public enum Gender
    {
        M,
        V,
        X
    }
}
