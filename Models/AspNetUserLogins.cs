//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyCongTacTroGiangKhoaCNTT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUserLogins
    {
        public string LoginProvIDer { get; set; }
        public string ProvIDerKey { get; set; }
        public string UserID { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
