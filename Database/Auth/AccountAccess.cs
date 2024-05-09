using AzerothConnect.Models.Account;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace AzerothConnect.Database.Auth;

[PrimaryKey("Id")]
[Table("account_access")]
public class AccountAccess
{
    [Column("id")]
    public int Id { get; set; }

    [Column("gmlevel")]
    public GMLevel Role { get; set; }

    [Column("RealmID")]
    public int RealmID { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }
}
