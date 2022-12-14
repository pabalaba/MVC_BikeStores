namespace MVC_BikeStores.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.Orders")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Order_Items = new HashSet<Order_Items>();
        }

        [Key]
        public int Order_Id { get; set; }

        public int? Customer_Id { get; set; }

        public byte Order_Status_Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime Order_Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime Required_Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Shipped_Date { get; set; }

        public int Store_Id { get; set; }

        public int Staff_Id { get; set; }

        public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Items> Order_Items { get; set; }

        public virtual Order_Status Order_Status { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Store Store { get; set; }

    }
}
