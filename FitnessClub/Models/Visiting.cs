//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FitnessClub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Visiting
    {
        public int idVisiting { get; set; }
        public int idSeasonTicket { get; set; }
        [DataType(DataType.Date), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public System.DateTime date { get; set; }
    
        public virtual SeasonTicket SeasonTicket { get; set; }
    }
}