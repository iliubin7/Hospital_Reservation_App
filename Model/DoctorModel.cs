﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Reservation_App.Model
{
    /// <summary>
    /// Class represents doctors table of DataBase.
    /// </summary>
    public class DoctorModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public SpecialityModel Speciality { get; set; }
    }
}
