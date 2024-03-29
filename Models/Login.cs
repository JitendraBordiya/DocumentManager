﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCUMENTMANAGER.Models;

public partial class Login
{
    [Key]
    public int LoginId { get; set; }

    [ForeignKey("user")]
    public long UserId { get; set; }

    [Required]
    public string LoginToken { get; set; }

    public DateTime? LoginDate { get; set; }

    public bool? IsLogin { get; set; }

    public virtual UserRegistration user { get; set; }
}