//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace salon_krasoti
{
    using System;
    using System.Collections.Generic;
    
    public partial class Appointments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Appointments()
        {
            this.Payments = new HashSet<Payments>();
        }
    
        public int AppointmentID { get; set; }
        public int ClientID { get; set; }
        public int EmployeeID { get; set; }
        public int ServiceID { get; set; }
        public System.DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }
        public Nullable<int> PromotionID { get; set; }
    
        public virtual Clients Clients { get; set; }
        public virtual Employees Employees { get; set; }
        public virtual Services Services { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual Promotions Promotions { get; set; }

        public string ClientFullName => Clients != null ? $"{Clients.FirstName} {Clients.LastName}" : string.Empty;
        public string EmployeeFullName => Employees != null ? $"{Employees.FirstName} {Employees.LastName}" : string.Empty;

    }
}
