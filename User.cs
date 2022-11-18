namespace AndreiWebAPI.Models
{
    public class User
    {
        public int Id { get; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool Administrator
        {
            get => Id == 1;
        }

        public User(int id, string name, string password, string email)
        {
            Id = id;
            Name = name;
            Password = password;
            Email = email;
        }
    }
}
