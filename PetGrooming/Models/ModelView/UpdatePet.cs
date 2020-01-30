using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetGrooming.Models.ModelView
{
    public class UpdatePet
    {
        public Pet Pet { get; set; }

        public ICollection<Species> Species { get; set; }
    }
}