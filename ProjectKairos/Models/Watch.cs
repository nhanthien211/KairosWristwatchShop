//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectKairos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Watch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Watch()
        {
            this.Comments = new HashSet<Comment>();
            this.Images = new HashSet<Image>();
            this.Modifications = new HashSet<Modification>();
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public int WatchID { get; set; }
        public string WatchCode { get; set; }
        public string WatchDescription { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int MovementID { get; set; }
        public int ModelID { get; set; }
        public bool WaterResistant { get; set; }
        public string BandMaterial { get; set; }
        public Nullable<double> CaseRadius { get; set; }
        public string CaseMaterial { get; set; }
        public System.DateTime PublishedTime { get; set; }
        public string PublishedBy { get; set; }
        public int Discount { get; set; }
        public bool LEDLight { get; set; }
        public bool Alarm { get; set; }
        public string Thumbnail { get; set; }
        public bool Status { get; set; }
        public int Guarantee { get; set; }
    
        public virtual Account Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Image> Images { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Modification> Modifications { get; set; }
        public virtual Movement Movement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual WatchModel WatchModel { get; set; }
    }
}
