namespace FastKart.Models.DTOs
{
    //public class UserDTO
    //{
    //    public readonly Guid Id;
    //    public readonly string Name;
    //    public readonly string Email;
    //    public readonly DateTime CreatedAt;
    //    public readonly bool Status;
    //    public readonly string Phone;
    //    public UserDTO(AppUser appUser)
    //    {
    //        Id = appUser.Id;
    //        Name = appUser.Name;
    //        Email = appUser.Email;
    //        CreatedAt = appUser.CreatedAt;
    //        Status = appUser.Status;
    //        Phone = appUser.Phone;
    //    }
    //}

    public class UserDTO
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public DateTime CreatedAt { get; }
        public bool Status { get; }
        public string Phone { get; }

        public UserDTO(AppUser appUser)
        {
            Id = appUser.Id;
            Name = appUser.Name;
            Email = appUser.Email;
            CreatedAt = appUser.CreatedAt;
            Status = appUser.Status;
            Phone = appUser.Phone;
        }
    }

    public class UserWithRoleNameDTO
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public DateTime CreatedAt { get; }
        public bool Status { get; }
        public string Phone { get; }
        public string Role { get; }
        public UserWithRoleNameDTO(AppUser appUser)
        {
            Id = appUser.Id;
            Name = appUser.Name;
            Email = appUser.Email;
            CreatedAt = appUser.CreatedAt;
            Status = appUser.Status;
            Phone = appUser.Phone;
            if (appUser.Role != null)
            {
                Role = appUser.Role.Name;
            }
            else
            {
                Role = string.Empty;
            }
        }
    }
}
