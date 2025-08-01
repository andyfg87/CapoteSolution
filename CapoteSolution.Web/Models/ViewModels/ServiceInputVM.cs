﻿using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ServiceInputVM : CapoteSolution.Models.Interface.IEntityInputModel<Service, System.Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Servicio")]
        public DateTime ServiceDate  { get; set; }

        [RequiredIf(nameof(ServiceReasonId), ServiceReason.Reasons.MonthlyCounter, ErrorMessage = "Contador B/N requerido")]
        [Display(Name = "Contador Negro")]
        public int? BlackCounter { get; set; }

        [Display(Name = "Contador Color")]
        public int? ColorCounter { get; set; }

        [Display(Name = "Cantidad de Toners")]
        [RequiredIf(nameof(ServiceReasonId), ServiceReason.Reasons.TonerChange)]
        public int? BlackTonerQty { get; set; }

        [Display(Name = "Número de Ticket")]
        public string TicketNumber { get; set; }

        [Required]
        [Display(Name = "Impresora")]
        public string CopierId { get; set; }

        [Required]
        [Display(Name = "Motivo")]
        public byte ServiceReasonId { get; set; }

        [Required]
        [Display(Name = "Técnico")]
        public Guid TechnicianId { get; set; }

        // Listas para dropdowns
        public SelectList? AvailableCopiers { get; set; }
        public SelectList? AvailableTechnicians { get; set; }
        public SelectList? ServiceReasons { get; set; }

        public Service Export()
        {
            var entity = new Service();

            Merge(entity);

            return entity;
        }

        public void Import(Service entity)
        {
            Id = entity.Id;
            ServiceDate = entity.Date;
            BlackCounter = entity.BlackCounter;
            ColorCounter = entity.ColorCounter;            
            BlackTonerQty = entity.BlackTonerQty;
            CopierId = entity.CopierId;
            ServiceReasonId = entity.ServiceReasonId;
            TechnicianId = entity.TechnicianId;
            TicketNumber = entity.TicketNumber;
        }

        public void Merge(Service entity)
        {
            entity.Date = ServiceDate;
            entity.BlackCounter = BlackCounter ?? entity.BlackCounter;
            entity.ColorCounter = ColorCounter ?? entity.ColorCounter;
            entity.BlackTonerQty = BlackTonerQty;
            entity.CopierId = CopierId;
            entity.ServiceReasonId = ServiceReasonId;
            entity.TechnicianId = TechnicianId;
            entity.TicketNumber = TicketNumber;
        }
    }
}
