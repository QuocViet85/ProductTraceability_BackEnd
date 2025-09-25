
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.DoanhNghiep.Models;

public class DoanhNghiepIdVaTenModel
{
    public Guid DN_Id { set; get; }
    public string DN_Ten { set; get; }
}