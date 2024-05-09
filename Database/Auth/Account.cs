using AzerothConnect.Models.Account;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace AzerothConnect.Database.Auth;

[PrimaryKey("Id")]
[Table("account")]
public class Account
{
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [Column("salt")]
    public byte[]? Salt { get; set; }
    [Column("verifier")]
    public byte[]? Verifier { get; set; }
    [Column("session_key")]
    public byte[]? SessionKey { get; set; }
    [Column("totp_secret")]
    public byte[]? TotpSecret { get; set; }

    [Column("email")]
    public string? Email { get; set; }
    [Column("reg_mail")]
    public string? RegisterEmail { get; set; }

    [Column("joindate")]
    public DateTimeOffset JoinDate { get; set; }
    [Column("last_ip")]
    public string? LastIP { get; set; }
    [Column("last_attempt_ip")]
    public string? LastAttemptIP { get; set; }
    [Column("failed_logins")]
    public int FailedLogins { get; set; }
    [Column("locked")]
    public bool? Locked { get; set; }
    [Column("lock_country")]
    public string? LockCountry { get; set; }
    [Column("last_login")]
    public DateTimeOffset? LastLogin { get; set; }
    [Column("online")]
    public bool Online { get; set; }
    [Column("expansion")]
    public Expansion Expansion { get; set; }

    [Column("mutetime")]
    public long MuteTime { get; set; }
    [Column("mutereason")]
    public string? MuteReason { get; set; }
    [Column("muteby")]
    public string? MuteBy { get; set; }

    [Column("locale")]
    public byte Locale { get; set; }

    [Column("os")]
    public string? OS { get; set; }

    public Account()
    {
        Email = string.Empty;
        RegisterEmail = string.Empty;
        LastIP = string.Empty;
        LastAttemptIP = string.Empty;
        LockCountry = string.Empty;
        Locked = false;
        LastLogin = null;
        Online = false;
        MuteReason = string.Empty;
        MuteBy = string.Empty;
        OS = string.Empty;
    }
}
