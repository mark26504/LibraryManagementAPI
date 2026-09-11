namespace LibraryManagement.Shared.Dtos.Books
{
    public record BookResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string ISBN { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime PublicationDate { get; init; }
        public int TotalCopies { get; init; }
        public int AvailableCopies { get; init; }
        public string? CoverImageUrl { get; init; }
        public CategoryResponse Category { get; init; } = null!;
        public IEnumerable<AuthorResponse> Authors { get; init; } =
            Enumerable.Empty<AuthorResponse>();
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public byte[] RowVersion { get; init; } = Array.Empty<byte>();
    }

}
