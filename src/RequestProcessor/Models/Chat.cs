using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RequestProcessor.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class Chat
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required]
        public Collection<Message> Messages { get; } = new Collection<Message>();

        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}