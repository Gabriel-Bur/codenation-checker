using System;
using System.ComponentModel.DataAnnotations;

namespace codenation.checker.Api.Domain
{
    public class CodenationUser
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
