using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }

        //public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
