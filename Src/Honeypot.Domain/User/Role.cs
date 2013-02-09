using System.ComponentModel;
using SharpArch.Domain.DomainModel;

namespace Honeypot.Domain
{
    public enum Role
    {
        [Description("Member")]
        Member = 1,
        [Description("Administrator")]
        Administrator
    }
}