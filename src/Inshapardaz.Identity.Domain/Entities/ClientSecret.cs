namespace Inshapardaz.Identity.Domain.Entities
{
    public class ClientSecret : Secret
    {
        public Client Client { get; set; }
    }
}