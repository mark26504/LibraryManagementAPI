namespace LibraryManagement.Shared.Dtos.BorrowingRecords
{
    public record BorrowingResponse
    {
        public Guid Id { get; init; }

        // Mapped automatically by AutoMapper's default matching
        public DateTime BorrowedAt { get; init; }
        public DateTime DueDate { get; init; }
        public DateTime? ReturnedAt { get; init; }

        // Mapped by your custom .ForMember rules!
        public string Book { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string User { get; init; } = string.Empty;
        public bool IsOverdue { get; init; }
    }
}
