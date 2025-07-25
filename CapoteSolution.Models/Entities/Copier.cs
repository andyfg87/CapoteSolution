﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Models.Entities
{
    public class Copier: IEntity<string>
    {       

        [Key]       
        public string Id { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }       
        

        [EmailAddress]
        public string MachineEmail { get; set; }

        public string IPAddress { get; set; }
        
        public string Comments { get; set; }

        // Claves foráneas      

        [ForeignKey("MachineModel")]
        public System.Guid MachineModelId { get; set; }
        

        // Relaciones       
        public MachineModel MachineModel { get; set; }        
        public virtual Contract Contract { get; set; }
    }
}
